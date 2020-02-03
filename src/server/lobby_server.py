import socket
from queue import Queue
from _thread import *
from data_structure import *

HOST = '127.0.0.1'
PORT = 3098


class Lobby:
    def __init__(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()

        self.matching_list = []         # 매칭 요청한 유저들 담을 큐 객체
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트

        self.start_server()             # 서버 시작

    # 서버 시작
    def start_server(self):
        print('Lobby server start')
        print('waiting new client..')
        #매칭 큐 쓰레드

        testidx = 1
        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            #addr[0] : IP,  addr[1] : port,  testidx : 유저 DB id
            self.user_list.append([client_socket, addr, testidx])
            testidx += 1
            # 유저 정보 리스트에 저장
            start_new_thread(self.client_thread, (self.user_list[-1],))     # 쓰레드 시작
        #self.server_socket.close()

    # 매칭 쓰레드
    def matching_queue_thread(self):
        room_num = 1
        while True:
            if len(self.user_list) >= 2:
                #유저 추출
                users = []
                users.append(self.user_list[0])
                users.append(self.user_list[1])
                self.user_list.remove(0)
                self.user_list.remove(1)
                self.matching_catch(users)

    def matching_catch(self, users):
        for user in users:
            # 각 클라이언트에게 매칭이 잡혔다는 메시지를 전달
            response = MatchingResponseData(PacketId.matching_response.value,
                                            MatchingPacketId.matching_catch,
                                            MatchingResult.success.value).serialize()
            user[0].send(response)

    #각 클라이언트 소켓 쓰레드
    def client_thread(self, user_socket):
        while True:
            try:
                #클라이언트에서 온 데이터 수신
                message = user_socket[0].recv(1024)
                if not message:
                    self.remove(user_socket)
                    break
                #packet_data = self.deserialize(message)        # 해독된 data
                packet_data = MatchingData(message).deserialize()
                print(packet_data)
                self.divide_process(packet_data, user_socket)
            except Exception as e:
                print(e)

    def divide_process(self, packet_data, user_socket):
        # 클라이언트로부터 매칭 시작 요청
        if packet_data[1] == MatchingPacketId.matching_request.value:
            print("매칭 요청")
            self.request_matching(packet_data, user_socket)

        # 클라이언트로부터 매칭 수락
        elif packet_data[1] == MatchingPacketId.matching_accept.value:
            print(user_socket[1][0] + ' ' + str(user_socket[1][1]) + ' 요청 수락')
            self.accept_matching(packet_data)

        # 클라이언트로부터 매칭 거절
        elif packet_data[1] == MatchingPacketId.matching_accept.value:
            self.reject_matching(packet_data)
            print("awd")

    def request_matching(self, packet_data, user_socket):
        self.matching_list.append(user_socket)        # 매칭 요청 유저 리스트에 삽입
        # response 전송
        print(PacketId.matching_response.value)
        response = MatchingResponseData(PacketId.matching_response.value,
                                        MatchingPacketId.matching_response,
                                        MatchingResult.success.value).serialize()
        user_socket[0].send(response)
        print("매칭 응답 전송")

    def accept_matching(self, packet_data):
        print("asd")

    def reject_matching(self, packet_data):
        print("asd")

    def remove(self, connection):
        if connection in self.user_list:
            self.user_list.remove(connection)
            print(connection[2].decode() + "님이 나가셨습니다.")

server = Lobby()
Lobby.start_server()