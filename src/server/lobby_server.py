import socket
from _thread import *
from data_structure import *
import time

HOST = '127.0.0.1'
PORT = 3098
TRY_MATCH = 0
RETRY_MATCH = 1


class Lobby:
    def __init__(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()

        self.matching_list = []         # 매칭 요청한 유저들 담을 큐 객체
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.room_num = 1
        try:
            self.start_server()
        except Exception as e:
            print(e)

    def start_matching_thread(self):
        print("start matching queue")
        start_new_thread(self.matching_queue_thread, ())

    # 서버 시작
    def start_server(self):
        self.start_matching_thread()        # 매칭 큐 쓰레드
        print('Lobby server start')
        print('waiting new client..')
        testidx = 1

        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            # addr[0] : IP,  addr[1] : port,  testidx : 유저 DB id
            self.user_list.append([client_socket, addr, testidx])
            testidx += 1
            # 유저 정보 리스트에 저장
            start_new_thread(self.client_thread, (self.user_list[-1],))     # 쓰레드 시작

    # 매칭 쓰레드
    def matching_queue_thread(self):
        while True:
            time.sleep(2)
            if len(self.matching_list) >= 2:
                #유저 추출
                users = []
                users.append(self.matching_list[0])
                del self.matching_list[0]
                users.append(self.matching_list[0])
                del self.matching_list[0]

                self.matching_catch(users)
                print("매칭 잡힘")

    def matching_catch(self, users):
        for my_info in users:
            # 각 클라이언트에게 매칭이 잡혔다는 메시지를 전달
            # 유저 리스트 자체를 전달
            for opponent_info in users:
                if my_info is not opponent_info:
                    response = MatchingResponseData(PacketId.matching_response.value,
                                                    MatchingPacketId.matching_catch.value,
                                                    MatchingResult.success.value,
                                                    self.room_num,
                                                    my_info[2],
                                                    opponent_info[2]).serialize()
                    my_info[0].send(response)
                    self.room_num += 1

    # 각 클라이언트 소켓 쓰레드
    def client_thread(self, user_socket):
        while True:
            try:
                # 클라이언트에서 온 데이터 수신
                message = user_socket[0].recv(1024)
                if not message:
                    self.remove(user_socket)
                    break
                self.divide_process(int.from_bytes(message[0:4], byteorder='big'), message, user_socket)
            except Exception as e:
                print(e)

    def divide_process(self, packet_id, message, user_socket):
        # 클라이언트로부터 매칭 시작 요청
        if packet_id == PacketId.matching_data.value:
            packet_data = MatchingData(message).deserialize()
            if packet_data[1] == MatchingPacketId.matching_request.value:
                print("매칭 요청")
                self.request_matching(user_socket, TRY_MATCH)
        # 클라이언트로부터 매칭 거절 요청
        elif packet_id == PacketId.matching_reject.value:
            print("reject~~!~!~!~!~!")
            packet_data = MatchingRejectData(message).deserialize()
            self.reject_matching(packet_data)

    # 매칭 등록
    def request_matching(self, user_socket, match_type):
        self.matching_list.append(user_socket)        # 매칭 요청 유저 리스트에 삽입
        # response 전송
        if match_type == TRY_MATCH:         # 매칭 요청으로 매칭 메세지 전송
            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_response.value,
                                            MatchingResult.success.value,
                                            0, 0, 0).serialize()
            user_socket[0].send(response)

        elif match_type == RETRY_MATCH:     # 상대방이 거절한 유저 재매칭 메세지 전송
            print("retry matching 진입")
            response = MatchingRetryData(PacketId.matching_retry.value, MatchingResult.success.value).serialize()
            user_socket[0].send(response)
        print("매칭 응답 전송")

    def accept_matching(self, packet_data, user_socket):
        response = MatchingResponseData(PacketId.matching_response.value,
                                        MatchingPacketId.matching_accept.value,
                                        MatchingResult.success.value,
                                        self.room_num).serialize()
        user_socket[0].send(response)
        print("매칭 수락 응답 전송")

    # 내 자신이 매칭 거절 누를시 상대방에게 재매치 메시지 전달
    def reject_matching(self, packet_data):
        print(str(packet_data[1]) + "매칭 취소")
        print("상대 아이디 : " + str(packet_data[2]))
        opponent_info = packet_data[2]          # 상대방 아이디
        self.retry_request_matching(opponent_info)

    def retry_request_matching(self, opponent_info):
        for user in self.user_list:
            print("유저 반복 : " + str(user[2]))
            if user[2] == opponent_info:
                print("제발 집가자")
                self.request_matching(user, RETRY_MATCH)

    def remove(self, connection):
        if connection in self.user_list:
            self.user_list.remove(connection)
            print(connection[2].decode() + "님이 나가셨습니다.")

server = Lobby()
