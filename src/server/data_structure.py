from enum import Enum


# 클라이언트에서 받는 요청 ID
class PacketId(Enum):
    matching_data = 3       #클라이언트에서는 게임에 있는 패킷도 있지만 매칭서버는 매칭만 신경쓰면 됨
    matching_response = 4


class MatchingPacketId(Enum):
    matching_request = 0
    matching_response = 1
    matching_catch = 2
    matching_accept = 3
    matching_reject = 4


# 클라이언트로 보내는 응답 ID
class MatchingResult(Enum):
    success = 0
    fail = 1


# 클라이언트로부터 받는 매칭 요청 패킷
class MatchingData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        print(self.message)
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        matching_packet_id = int.from_bytes(self.message[4:8], byteorder='big')
        index = int.from_bytes(self.message[8:12], byteorder='big')
        room_num = int.from_bytes(self.message[12:16], byteorder='big')
        packet = [packet_id, matching_packet_id, index, room_num]

        return packet


# 클라이언트로 보낼 매칭 요청 응답 패킷
class MatchingResponseData:
    def __init__(self, packet_id, matching_request, matching_result):
        self.data = [packet_id, matching_request, matching_result]
        #바이트로 변환해야함

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + self.data[1].to_bytes(4, byteorder='big') + self.data[2].to_bytes(4, byteorder='big')
        return packet


# 클라이언트로 보낼 매칭 성립 응답 패킷
#class MatchingCatchData:
#    def __init__(self):
#packet = [1, 1]
#a = packet[0].to_bytes(4, byteorder='big')
#print(a)
