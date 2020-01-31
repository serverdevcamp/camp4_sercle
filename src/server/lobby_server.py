import socket
from _thread import *
import marshal

HOST = '127.0.0.1'
PORT = 3098

class Lobby:
    def __init__(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()
        # 서버에 접속한 유저 소켓 리스트 저장
        self.user_list = []
        print('Lobby server start')

    def start_server(self):
        while True:
            print('waiting new client..')
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            self.user_list.append([client_socket])
            # 쓰레드 시작
            start_new_thread(self.client_thread, (self.user_list[-1],))
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
            except Exception as e:
                print(e)
                continue

    def remove(self, connection):
        if connection in self.user_list:
            self.user_list.remove(connection)
            print(connection[2].decode() + "님이 나가셨습니다.")




