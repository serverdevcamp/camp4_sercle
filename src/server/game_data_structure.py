from enum import Enum


class PacketId(Enum):
    game_join = 10
    game_end = 12


class GamePacketId(Enum):
    normal_end = 0
    opponent_end = 1


class GameJoinData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        user_id = int.from_bytes(self.message[4:8], byteorder='big')
        room_num = int.from_bytes(self.message[8:12], byteorder='big')

        packet = [packet_id, user_id, room_num]
        return packet


class GameEndData:
    def __init__(self, packet_id, game_packet_id):
        self.data = [packet_id, game_packet_id]

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big')
        return packet
