import asyncio
from _thread import *
from matching_data_structure import *
import time

HOST = '0.0.0.0'
PORT = 3098
TRY_MATCH = 0
RETRY_MATCH = 1
FIRST_PLAYER = 1
SECOND_PLAYER = 2


class Lobby:
    def __init__(self):
        self.matching_list = []         # 매칭 요청한 유저들 담을 큐 객체
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.room_num = 1
        self.accept_dic = {}

        print("start matching queue")
        #start_new_thread(self.matching_queue_thread, ())
        asyncio.run(self.main())

    # 매칭 잡힘
    async def matching_catch_thread(self, my_write, opponent_write):
        try:
            print("유저id : " + str(my_write[1]) + str(opponent_write[1]))
            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_catch.value,
                                            MatchingResult.success.value,
                                            my_write[1]).serialize()
            my_write[0].write(response)
            await my_write[0].drain()

            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_catch.value,
                                            MatchingResult.success.value,
                                            opponent_write[1]).serialize()
            opponent_write[0].write(response)
            await opponent_write[0].drain()

            times = 0
            while times < 20:
                await asyncio.sleep(0.3)
                # 둘다  매칭 수락함.
                print("시간 대기중 : " + str(self.accept_dic[my_write[1]]) + "  " + str(self.accept_dic[opponent_write[1]]))
                if self.accept_dic[my_write[1]] == 1 and self.accept_dic[opponent_write[1]] == 1:
                    await self.accept_response(my_write, opponent_write)
                    return 0
                times += 1

            # 10초가 지나면
            # 내가 수락 상대방 거절
            if self.accept_dic[my_write[1]] == 1 and (self.accept_dic[opponent_write[1]] == 0 or
                                                      self.accept_dic[opponent_write[1]] == -1):
                await self.retry_request_matching(my_write[0])
                await self.reject_response(opponent_write[0])

            # 내가 거절 상대방 수락
            elif self.accept_dic[opponent_write[1]] == 1 and (self.accept_dic[my_write[1]] == 0 or
                                                              self.accept_dic[my_write[1]] == -1):
                await self.retry_request_matching(opponent_write[0])
                await self.reject_response(my_write[0])
            # 둘다 거절
            else:
                await self.reject_response(my_write[0])
                await self.reject_response(opponent_write[0])
        except Exception as e:
            print("매칭 도중 유저 나감")
            self.matching_remove(my_write, opponent_write)

    # 매칭 쓰레드
    async def matching_queue_thread(self):
        while True:
            await asyncio.sleep(0.1)
            time.sleep(0.5)
            if len(self.matching_list) >= 2:
                #유저 추출
                users = []
                users.append(self.matching_list[0])
                del self.matching_list[0]
                users.append(self.matching_list[0])
                del self.matching_list[0]

                #매칭된 두 유저 dic 0으로 초기화 -> 클라이언트로부터 수락 요청시 1, 거절 요청시 -1 처리
                self.accept_dic[users[0][1]] = 0
                self.accept_dic[users[1][1]] = 0
                asyncio.create_task(self.matching_catch_thread(users[0], users[1]))     #0: writer 1: id
                print("매칭 잡힘")

    # 상대방 거절 이후 재매칭
    async def retry_request_matching(self, retry_user):
        for user in self.user_list:         #모든 유저 리스트
            if user[1] == retry_user:
                await self.request_matching(user[1], user[2], RETRY_MATCH)

    # 매칭 둘다 완료
    async def accept_response(self, my_write, opponent_write):
        message = MatchingCompleteData(PacketId.matching_complete.value,
                                       self.room_num,
                                       my_write[1],
                                       FIRST_PLAYER).serialize()
        my_write[0].write(message)
        message = MatchingCompleteData(PacketId.matching_complete.value,
                                       self.room_num,
                                       opponent_write[1],
                                       SECOND_PLAYER).serialize()
        opponent_write[0].write(message)
        self.room_num += 1      #각 방번호 부여

    # 매칭 거절
    async def reject_response(self, writer):
        #매칭 잡힌 후 10초뒤에 전달되는 메시지
        message = MatchingRejectData(PacketId.matching_reject.value, MatchingResult.success.value).serialize()
        writer.write(message)
        await writer.drain()

    # 매칭 소켓 핸들러
    async def lobby_handle(self, reader, writer):
        addr = writer.get_extra_info('peername')  # 0: IP 1: PORT
        print(str(addr[0]) + " " + str(addr[1]) + "connect")
        user_id = await reader.read(100)
        user_id = user_id.decode()
        print("userid : " + str(int(user_id)))
        self.user_list.append([reader, writer, int(user_id)])

        while True:
            try:
                message = await reader.read(100)
                if not message:
                    self.remove(int(user_id))
                    break
                await self.divide_process(int.from_bytes(message[0:4], byteorder='big'), message, writer, int(user_id))
            except Exception as e:
                print(e)
                #remove
                break

    # 메세지 처리
    async def divide_process(self, packet_id, message, writer, user_id):
        # 클라이언트로부터 매칭 시작 요청
        if packet_id == PacketId.matching_data.value:
            packet_data = MatchingData(message).deserialize()
            if packet_data[1] == MatchingPacketId.matching_request.value:
                print(str(user_id) + " : 매칭 요청")
                await self.request_matching(writer, user_id, TRY_MATCH)
        # 클라이언트로부터 매칭 수락, 거절 여부 메시지
        elif packet_id == PacketId.matching_decision.value:

            packet_data = MatchingDecisionData(message).deserialize()
            #수락 눌렀을 시
            if packet_data[1] == MatchingDecision.matching_accept.value:
                print("수락")
                self.accept_matching(packet_data)
            elif packet_data[1] == MatchingDecision.matching_reject.value:
                print("거절")
                self.reject_matching(packet_data)
        # 클라이언트로부터 매칭 취소
        elif packet_id == PacketId.matching_cancel.value:
            self.cancel_matching(user_id)

    # 매칭 등록
    async def request_matching(self, writer, user_id, match_type):
        self.matching_list.append([writer, user_id])        # 매칭 요청 유저 리스트에 삽입
        # response 전송
        if match_type == TRY_MATCH:                   # 매칭 요청으로 매칭 메세지 전송
            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_response.value,
                                            MatchingResult.success.value,
                                            user_id).serialize()
            print("matching regist")
            writer.write(response)
            await writer.drain()
        elif match_type == RETRY_MATCH:              # 상대방이 거절한 유저 재매칭 메세지 전송
            response = MatchingRetryData(PacketId.matching_retry.value, MatchingResult.success.value).serialize()
            writer.write(response)
            await writer.drain()

        print(str(user_id) + " : 매칭 응답 전송")

    # 매칭 수락
    def accept_matching(self, packet_data):
        self.accept_dic[packet_data[2]] = 1     #수락했다고 알림
        print(str(packet_data[2]) + " : 매칭 수락")

    # 매칭 거절
    def reject_matching(self, packet_data):
        self.accept_dic[packet_data[2]] = -1    #거절했다 알림
        print(str(packet_data[2]) + " : 매칭 거절")

    # 매칭 취소
    def cancel_matching(self, user_id):
        for user in self.matching_list:
            if user[1] is user_id:
                self.matching_list.remove(user)
                print(str(user_id) + " : 매칭 취소 처리")

    async def main(self):
        asyncio.create_task(self.matching_queue_thread())
        server = await asyncio.start_server(self.lobby_handle, HOST, PORT)

        address = server.sockets[0].getsockname()
        print({address})

        print('Lobby server start')
        print('waiting new client..')

        async with server:
            await server.serve_forever()

    # 접속 종료 유저 리스트 제거
    def remove(self, user_id):
        for user in self.matching_list:
            if user[1] == user_id:
                print(str(user[1]) + "님이 매칭 리스트에서 나가셨습니다.")
                self.matching_list.remove(user)
                break
        for user in self.user_list:
            if user[2] == user_id:
                print(str(user[2]) + "님이 나가셨습니다.")
                self.user_list.remove(user)
                break

    def matching_remove(self, my, opponent):
        for user in self.matching_list:
            if user[1] == my or user[1] == opponent:
                print(str(user[1]) + "님이 매칭 리스트에서 나가셨습니다.")
                self.matching_list.remove(user)
                break
server = Lobby()