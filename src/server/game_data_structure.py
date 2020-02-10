from enum import Enum


class PacketId(Enum):
    game_join = 1


class GameJoin:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        user_id = int.from_bytes(self.message[4:8], byteorder='big')
        room_num = int.from_bytes(self.message[8:12], byteorder='big')

        packet = [packet_id, user_id, room_num]
        return packet
