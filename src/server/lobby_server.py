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

        self.matching_list = []   # 매칭 요청한 유저들 담을 큐 객체
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.start_server()             # 서버 시작

    # 서버 시작
    def start_server(self):
        print('Lobby server start')
        print('waiting new client..')
        testidx = 1
        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            self.user_list.append([client_socket, addr, testidx])
            testidx += 1
            # 유저 정보 리스트에 저장
            start_new_thread(self.client_thread, (self.user_list[-1],))     # 쓰레드 시작
        #self.server_socket.close()

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
        if packet_data[0] is MatchingPacketId.matching_request.value:
            print("매칭 요청")
            self.request_matching(packet_data, user_socket)

        # 클라이언트로부터 매칭 수락
        elif packet_data[0] is MatchingPacketId.matching_accept.value:
            self.accept_matching(packet_data)
            print("asd")

        # 클라이언트로부터 매칭 거절
        elif packet_data[0] is MatchingPacketId.matching_accept.value:
            self.reject_matching(packet_data)
            print("awd")

    def request_matching(self, packet_data, user_socket):
        self.matching_list.append(packet_data[1])        # 매칭 요청 유저 리스트에 삽입
        # response 전송
        response = MatchingResponseData(packet_data[0], MatchingResult.success)
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