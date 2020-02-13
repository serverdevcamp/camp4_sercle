import socket
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
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()

        self.matching_list = []         # 매칭 요청한 유저들 담을 큐 객체
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.room_num = 1
        self.accept_dic = {}

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

        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            user_id = client_socket.recv(20)
            print("userid : " + str(int(user_id)))
            # addr[0] : IP,  addr[1] : port,  testidx : 유저 DB id
            self.user_list.append([client_socket, addr, int(user_id)])      #로그인 전용
            # 유저 정보 리스트에 저장
            start_new_thread(self.client_thread, (self.user_list[-1],))     # 쓰레드 시작

    # 매칭 쓰레드
    def matching_queue_thread(self):
        while True:
            time.sleep(0.5)
            if len(self.matching_list) >= 2:
                #유저 추출
                users = []
                users.append(self.matching_list[0])
                del self.matching_list[0]
                users.append(self.matching_list[0])
                del self.matching_list[0]

                #매칭된 두 유저 dic 0으로 초기화 -> 클라이언트로부터 수락 요청시 1, 거절 요청시 -1 처리
                self.accept_dic[users[0][2]] = 0
                self.accept_dic[users[1][2]] = 0
                start_new_thread(self.matching_catch_thread, (users[0], users[1]))
                print("매칭 잡힘")

    # 매칭 둘다 완료
    def accept_response(self, my_socket, opponent_socket):
        message = MatchingCompleteData(PacketId.matching_complete.value,
                                       self.room_num,
                                       my_socket[2],
                                       FIRST_PLAYER).serialize()
        my_socket[0].send(message)
        message = MatchingCompleteData(PacketId.matching_complete.value,
                                       self.room_num,
                                       opponent_socket[2],
                                       SECOND_PLAYER).serialize()
        opponent_socket[0].send(message)
        self.room_num += 1      #각 방번호 부여

    # 매칭 거절
    def reject_response(self, user_socket):
        #매칭 잡힌 후 10초뒤에 전달되는 메시지
        message = MatchingRejectData(PacketId.matching_reject.value, MatchingResult.success.value).serialize()
        user_socket[0].send(message)

    # 매칭 잡힘
    def matching_catch_thread(self, my_socket, opponent_socket):
        print("유저id : " + str(my_socket[2]) + str(opponent_socket[2]))
        response = MatchingResponseData(PacketId.matching_response.value,
                                        MatchingPacketId.matching_catch.value,
                                        MatchingResult.success.value,
                                        my_socket[2]).serialize()
        my_socket[0].send(response)
        response = MatchingResponseData(PacketId.matching_response.value,
                                        MatchingPacketId.matching_catch.value,
                                        MatchingResult.success.value,
                                        opponent_socket[2]).serialize()
        opponent_socket[0].send(response)

        times = 0
        while times < 20:
            # 둘다  매칭 수락함.
            print("시간 대기중 : " + str(self.accept_dic[my_socket[2]]) + "  " + str(self.accept_dic[opponent_socket[2]]))
            if self.accept_dic[my_socket[2]] == 1 and self.accept_dic[opponent_socket[2]] == 1:
                self.accept_response(my_socket, opponent_socket)
                return 0
            times += 1
            time.sleep(0.5)

        #10초가 지나면
        #내가 수락 상대방 거절
        if self.accept_dic[my_socket[2]] == 1 and (self.accept_dic[opponent_socket[2]] == 0 or
                                                   self.accept_dic[opponent_socket[2]] == -1):
            self.retry_request_matching(my_socket)
            self.reject_response(opponent_socket)
            #retry 메시지
        #내가 거절 상대방 수락
        elif self.accept_dic[opponent_socket[2]] == 1 and (self.accept_dic[my_socket[2]] == 0 or
                                                           self.accept_dic[my_socket[2]] == -1):
            self.retry_request_matching(opponent_socket)
            self.reject_response(my_socket)
        #둘다 거절
        else:
            self.reject_response(my_socket)
            self.reject_response(opponent_socket)

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

    # 메세지 처리
    def divide_process(self, packet_id, message, user_socket):
        # 클라이언트로부터 매칭 시작 요청
        if packet_id == PacketId.matching_data.value:
            packet_data = MatchingData(message).deserialize()
            if packet_data[1] == MatchingPacketId.matching_request.value:
                print(str(user_socket[2]) + " : 매칭 요청")
                self.request_matching(user_socket, TRY_MATCH)
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
            self.cancel_matching(user_socket)

    # 매칭 취소
    def cancel_matching(self, cancel_user_socket):
        for user in self.matching_list:
            if user == cancel_user_socket:
                self.matching_list.remove(cancel_user_socket)
                print(str(cancel_user_socket[2]) + " : 매칭 취소 처리")

    # 상대방 거절 이후 재매칭
    def retry_request_matching(self, retry_user_socket):
        for user in self.user_list:         #모든 유저 리스트
            if user == retry_user_socket:
                self.request_matching(user, RETRY_MATCH)

    # 매칭 등록
    def request_matching(self, user_socket, match_type):
        self.matching_list.append(user_socket)        # 매칭 요청 유저 리스트에 삽입
        # response 전송
        if match_type == TRY_MATCH:                   # 매칭 요청으로 매칭 메세지 전송
            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_response.value,
                                            MatchingResult.success.value,
                                            user_socket[2]).serialize()
            user_socket[0].send(response)

        elif match_type == RETRY_MATCH:              # 상대방이 거절한 유저 재매칭 메세지 전송
            response = MatchingRetryData(PacketId.matching_retry.value, MatchingResult.success.value).serialize()
            user_socket[0].send(response)
        print(str(user_socket[2]) + " : 매칭 응답 전송")

    # 매칭 수락
    def accept_matching(self, packet_data):
        self.accept_dic[packet_data[2]] = 1     #수락했다고 알림
        print(str(packet_data[2]) + " : 매칭 수락")

    # 매칭 거절
    def reject_matching(self, packet_data):
        self.accept_dic[packet_data[2]] = -1    #거절했다 알림
        print(str(packet_data[2]) + " : 매칭 거절")

    # 접속 종료 유저 리스트 제거
    def remove(self, connection):
        if connection in self.matching_list:
            self.matching_list.remove(connection)
        if connection in self.user_list:
            self.user_list.remove(connection)
            print(str(connection[2]) + "님이 나가셨습니다.")


server = Lobby()
