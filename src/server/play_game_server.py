import socket
from _thread import *
from game_data_structure import *
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

        self.game_wait_dic = {}         # 유저를 기다리는 리스트
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.game_data_dic = {}

        try:
            self.start_server()
        except Exception as e:
            print(e)

    # 서버 시작
    def start_server(self):
        print('Game server start')
        start_new_thread(self.game_wait_thread, ())
        print("wait")

        while True:
            client_socket, addr = self.server_socket.accept()  # 소켓
            print(addr[0] + ' ' + str(addr[1]) + ' connected')

            user_info = client_socket.recv(20)
            game_user = GameJoinData(user_info).deserialize()

            print("userid : " + str(game_user[1]) + "roomNUm : " + str(game_user[2]))
            # 로그인 전용# addr[0] : IP,  addr[1] : port,  testidx : 유저 DB id
            self.user_list.append([client_socket, addr, game_user[1], game_user[2]])
            # 유저 정보 리스트에 저장
            self.room_num_check(game_user[2])

    def room_num_check(self, room_num):
        if room_num in self.game_wait_dic.keys():
            self.game_wait_dic[room_num] += 1
        else:
            self.game_wait_dic[room_num] = 1

    # 각 클라이언트 소켓 쓰레드
    def client_thread(self, my_socket, opponent_socket):
        room_num = my_socket[3]
        self.game_data_dic[room_num] = {}
        self.game_data_dic[room_num]['data'] = {}               #공통 데이터
        self.game_data_dic[room_num][my_socket[2]] = {}         #나의 데이터
        self.game_data_dic[room_num][opponent_socket[2]] = {}   #상대 데이터

        while True:
            try:
                # 클라이언트에서 온 데이터 수신
                message = my_socket[0].recv(1024)
                if not message:
                    self.remove(my_socket)
                    self.opponent_remove(opponent_socket)
                    break
                self.divide_packet(message, my_socket, opponent_socket, room_num)
            except Exception as e:
                print(str(e) + "zxczxc")
                break

    def divide_packet(self, message, my_socket, opponent_socket, room_num):
        packet_id = int.from_bytes(message[0:4], byteorder='big')

        if packet_id == PacketId.select_skill.value or packet_id == PacketId.game_finish.value:
            my_socket[0].send(message)
            opponent_socket[0].send(message)
        else:
            opponent_socket[0].send(message)

    def game_wait_thread(self):
        while True:
            delete_room_num = -1
            for room_num, user_num in list(self.game_wait_dic.items()):
                if user_num == 2:
                    delete_room_num = room_num
                    play_user = []
                    for user in self.user_list:
                        if user[3] == room_num:
                            play_user.append(user)
                    start_new_thread(self.client_thread, (play_user[0], play_user[1]))
                    start_new_thread(self.client_thread, (play_user[1], play_user[0]))
                    print("방 : " + str(room_num) + " 게임 시작")

            if delete_room_num != -1:
                self.game_wait_dic.pop(delete_room_num)

    def remove(self, user_socket):
        if user_socket in self.user_list:
            self.user_list.remove(user_socket)
            print(str(user_socket[2]) + "님이 나가셨습니다.")

    def opponent_remove(self, user_socket):
        if user_socket in self.user_list:
            self.user_list.remove(user_socket)
            message = GameEndData(PacketId.game_end.value, GamePacketId.opponent_end.value).serialize()
            user_socket[0].send(message)
            print(str(user_socket[2]) + "님 로비로 이동")


start = Game()
