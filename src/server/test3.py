import asyncio
from game_data_structure import *
from _thread import *

HOST = '0.0.0.0'
PORT = 1000
GAME_START = 0
GAME_END = 1

class Game:
    def __init__(self):
        self.game_wait_dic = {}         # 유저를 기다리는 리스트
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.game_data_dic = {}
        self.game_start_dic = {}
        self.game_end_dic = {}
        asyncio.run(self.main())

    async def game_handle(self, reader, writer):
        #소켓 연결 확인
        addr = writer.get_extra_info('peername')  # 0: IP 1: PORT
        await asyncio.sleep(0.1)
        print(str(addr[0]) + " " + str(addr[1]) + "connect")

        # 방 정보 가져오기
        user_info = await reader.read(100)
        my_user = GameJoinData(user_info).deserialize()

        # 방 번호 저장
        room_num = my_user[2]

        # 내 정보 저장
        my_id = my_user[1]
        self.user_list.append([writer, my_id, room_num])
        print("userid : " + str(my_id) + "roomNUm : " + str(room_num))

        # 상대가 게임에 접속하면 상대방의 정보를 가져옴
        self.room_num_check(my_user[2])
        opponent_user = await self.game_wait(my_id, room_num)
        opponent_writer = opponent_user[0]
        opponent_id = opponent_user[1]

        self.game_data_dic[room_num] = {}
        self.game_data_dic[room_num]['monster'] = {}        # 몬스터 정보 담을 Dic
        #self.game_data_dic[room_num]['data'] = {}
        #self.game_data_dic[room_num][my_id] = {}
        #self.game_data_dic[room_num][opponent_id] = {}

        print("유저 : " + str(my_id) + " " + str(opponent_id) + " 게임 시작")
        while True:
            try:
                message = await reader.read(40)
                if not message:
                    self.game_wait_dic.pop(room_num)
                    self.game_end_dic[room_num] = GAME_END
                    self.remove(my_id)
                    await self.opponent_remove(opponent_id)
                    break
                await self.divide_packet(message, writer, opponent_writer, room_num)
            except Exception as e:
                print(str(e))
                break

    async def divide_packet(self, message, my_writer, opponent_writer, room_num):
        packet_id = int.from_bytes(message[0:4], byteorder='big')
        if packet_id == PacketId.select_skill.value:
            print("스킬선택 : " + str(message))
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()

        elif packet_id == PacketId.game_finish.value:
            print("게임종료 여부 : " + str(int.from_bytes(message[4:8], byteorder='big')))
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()

        elif packet_id == PacketId.skill_data.value:
            print("스킬 사용 : " + str(message))
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()

        elif packet_id == PacketId.skill_hit.value:
            print("스킬 피격 : " + str(message))
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()
        elif packet_id == PacketId.spawn_robots.value:
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()
        elif packet_id == PacketId.game_start.value:
            # 게임씬 접속했다고 알림
            print("게임씬 입장 완료 : " + str(int.from_bytes(message[4:8], byteorder='big')))
            my_writer.write(message)
            await my_writer.drain()
            opponent_writer.write(message)
            await opponent_writer.drain()
            #await self.game_start_num_check(room_num, my_writer)
        else:
            print("쓰레기 : " + str(int.from_bytes(message[0:4], byteorder='big')) + str(int.from_bytes(message[4:8], byteorder='big')))
            #opponent_writer.write(message)

    async def create_monster(self, room_num, my_writer, opponent_writer):
        num = 0
        while self.game_end_dic[room_num] != GAME_END:
            await asyncio.sleep(10.0)
            print(str(room_num) + "번 방 몬스터 생성")
            message = SpawnRobotData(PacketId.spawn_robots.value, 0).serialize()
            for i in range(1, 10):
                await asyncio.sleep(0.5)
                my_writer.write(message)
                await my_writer.drain()
                opponent_writer.write(message)
                await opponent_writer.drain()
            num = num + 1
            if num == 5:
                break
            # 게임이 끝났다는 플래그를 넣어야함.
        self.game_end_dic.pop(room_num)     # dic 제거

    async def game_start_num_check(self, room_num, my_writer):
        if room_num in self.game_start_dic.keys():
            self.game_start_dic[room_num].append(my_writer)
            # 미니언 생성 task
            asyncio.create_task(self.create_monster(room_num,
                                                    self.game_start_dic[room_num][0],
                                                    self.game_start_dic[room_num][1]))
            # 게임이 종 플래그
            self.game_end_dic[room_num] = GAME_START
            # 게임 스타트 dic 제거
            self.game_start_dic.pop(room_num)
        else:
            self.game_start_dic[room_num] = []
            self.game_start_dic[room_num].append(my_writer)

    async def game_wait(self, user_id, room_num):
        while True:
            await asyncio.sleep(0.5)
            #print("game_wait")
            #delete_room_num = -1
            if room_num in self.game_wait_dic.keys():
                if self.game_wait_dic[room_num] == 2:
                    #delete_room_num = room_num
                    for user in self.user_list:
                        if user[2] == room_num and user[1] != user_id:      #각 유저의 상대 유저 데이터를 각 유저 소켓으 가져온다.
                            return user

            #if delete_room_num != -1:
            #   self.game_wait_dic.pop(delete_room_num)            remove에 추가

    def room_num_check(self, room_num):
        if room_num in self.game_wait_dic.keys():
            self.game_wait_dic[room_num] += 1
        else:
            self.game_wait_dic[room_num] = 1

    async def main(self):
        server = await asyncio.start_server(self.game_handle, HOST, PORT)
        address = server.sockets[0].getsockname()
        print({address})
        print('Game server start')

        async with server:
            await server.serve_forever()

    def remove(self, user_id):
        for user in self.user_list:
            if user[1] == user_id:
                print(str(user[1]) + "님이 나가셨습니다.")
                self.user_list.remove(user)
                break

    async def opponent_remove(self, opponent_id):
        for user in self.user_list:
            if opponent_id == user[1]:
                message = GameEndData(PacketId.game_end.value, GamePacketId.opponent_end.value).serialize()
                user[0].write(message)
                print(str(opponent_id) + "님 로비로 이동")
                self.user_list.remove(user)
                break

start = Game()