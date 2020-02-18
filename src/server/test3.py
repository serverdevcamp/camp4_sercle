import asyncio
from game_data_structure import *
from _thread import *

HOST = '0.0.0.0'
PORT = 1000


class Game:
    def __init__(self):
        self.game_wait_dic = {}         # 유저를 기다리는 리스트
        self.user_list = []             # 서버에 접속한 유저 소켓 저장할 리스트
        self.game_data_dic = {}
        asyncio.run(self.main())

    async def game_handle(self, reader, writer):
        print("zzz")
        addr = writer.get_extra_info('peername')  # 0: IP 1: PORT
        await asyncio.sleep(0.1)
        print(str(addr[0]) + " " + str(addr[1]) + "connect")

        user_info = await reader.read(100)
        my_user = GameJoinData(user_info).deserialize()     #my_user -> GameJoinData 패
        my_id = my_user[1]
        room_num = my_user[2]
        print("userid : " + str(my_id) + "roomNUm : " + str(room_num))

        self.user_list.append([writer, my_id, room_num])
        self.room_num_check(my_user[2])
        opponent_user = await self.game_wait(my_id, room_num)
        opponent_writer = opponent_user[0]
        opponent_id = opponent_user[1]

        self.game_data_dic[room_num] = {}
        self.game_data_dic[room_num]['data'] = {}
        self.game_data_dic[room_num][my_id] = {}
        self.game_data_dic[room_num][opponent_id] = {}

        print("유저 : " + str(my_id) + " " + str(opponent_id) + " 게임 시작")
        while True:
            try:
                message = await reader.read(100)
                if not message:
                    break
                    #self.remove()
                await self.divide_packet(message, writer, opponent_writer)
            except Exception as e:
                print(str(e))
                break

    async def divide_packet(self, message, my_writer, opponent_writer):
        packet_id = int.from_bytes(message[0:4], byteorder='big')
        print(str(message))
        if packet_id == PacketId.select_skill.value:
            print("스킬선택 : " + str(int.from_bytes(message[8:12], byteorder='big')))
            my_writer.write(message)
            opponent_writer.write(message)
        elif packet_id == PacketId.game_finish.value:
            print("게임종료 여부 : " + str(int.from_bytes(message[4:8], byteorder='big')))
            my_writer.write(message)
            opponent_writer.write(message)
        elif packet_id == PacketId.skill_data.value:
            print("스킬 사용 : " + str(int.from_bytes(message[4:8], byteorder='big')) +
                  " 진영에서 " + str(int.from_bytes(message[12:16], byteorder='big')) + " 사용.")
            my_writer.write(message)
            opponent_writer.write(message)
        elif packet_id == PacketId.skill_hit.value:
            print("스킬 피격 : " + str(int.from_bytes(message[8:12], byteorder='big')))
            my_writer.write(message)
            opponent_writer.write(message)
        elif packet_id == PacketId.game_start.value:
            print("게임씬 입장 완료 : " + str(int.from_bytes(message[4:8], byteorder='big')))
            my_writer.write(message)
            opponent_writer.write(message)
        #else:
        #    opponent_writer.write(message)

    async def game_wait(self, user_id, room_num):
        while True:
            await asyncio.sleep(0.5)
            #print("game_wait")
            #delete_room_num = -1
            if room_num in self.game_wait_dic.keys():
                if self.game_wait_dic[room_num] == 2:
                    #delete_room_num = room_num
                    for user in self.user_list:
                        if user[2] == room_num and user[1] != user_id:      #방번호가
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

start = Game()