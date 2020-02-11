import socket
from _thread import *
from ..game_data_structure import *
import time

HOST = '0.0.0.0'
PORT = 1000


class Game:
    def __init__(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()

        self.game_wait_dic = {}         # 유저를 기다리는 리스
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트

        try:
            self.start_server()
        except Exception as e:
            print(e)

    # 서버 시작
    def start_server(self):
        print('Game server start')
        start_new_thread(self.game_wait_thread, ())

        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            user_info = client_socket.recv(20)
            game_user = GameJoin(user_info).deserialize()

            print("userid : " + str(game_user[1]) + "roomNUm : " + str(game_user[2]))
            self.user_list.append([client_socket, addr, game_user[1], game_user[2]])      # 로그인 전용# addr[0] : IP,  addr[1] : port,  testidx : 유저 DB id
            # 유저 정보 리스트에 저장
            self.room_num_check(game_user[2])
            # 쓰레드 시작

    def room_num_check(self, room_num):
        if room_num in self.game_wait_dic.keys():
            self.game_wait_dic[room_num] += 1
        else:
            self.game_wait_dic[room_num] = 1

    # 각 클라이언트 소켓 쓰레드
    def client_thread(self, my_socket, opponent_socket):
        while True:
            try:
                # 클라이언트에서 온 데이터 수신
                message = my_socket[0].recv(1024)
                if not message:
                    self.remove(my_socket)
                    break
                opponent_socket[0].send(message)
            except Exception as e:
                print(e)

    def game_wait_thread(self):
        while True:
            for room_num, user_num in self.game_wait_dic.items():
                if user_num == 2:
                    play_user = []
                    for user in self.user_list:
                        if user[3] == room_num:
                            play_user.append(user)
                    start_new_thread(self.client_thread, play_user[0], play_user[1])
                    start_new_thread(self.client_thread, play_user[1], play_user[0])
                    self.game_wait_dic.pop(room_num)

    def remove(self, connection):
        if connection in self.user_list:
            self.user_list.remove(connection)
            print(str(connection[2]) + "님이 나가셨습니다.")