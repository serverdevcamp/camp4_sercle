from enum import Enum


# 클라이언트에서 받는 요청 ID
class MatchingPacketId(Enum):
    matching_request = 0
    matching_accept = 1
    matching_reject = 2


# 클라이언트로 보내는 응답 ID
class MatchingResult(Enum):
    success = 0
    fail = 1


# 클라이언트로부터 받는 패킷
class MatchingData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        matching_packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        index = int.from_bytes(self.message[4:8], byteorder='big')
        room_num = int.from_bytes(self.message[8:12], byteorder='big')
        packet = [matching_packet_id, index, room_num]
        return packet


# 클라이언트로 보낼 패킷
class MatchingResponseData:
    def __init__(self, matching_result, matching_request):
        self.data = [matching_result, matching_request]
        #바이트로 변환해야함

    def serialize(self):
        packet = self.data[0].to_bytes(2, byteorder='big') + self.data[0].to_bytes(2, byteorder='big')
        return packet

#packet = [1, 1]
#a = packet[0].to_bytes(2, byteorder='big')
#print(a)
# print(0 == MatchingPacketId.matching_request.value)
# a = (1).to_bytes(2, byteorder='big')
# b = (2).to_bytes(2, byteorder='big')
# print(a+b)