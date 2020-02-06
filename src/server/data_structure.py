from enum import Enum


# 클라이언트에서 받는 요청 ID
class PacketId(Enum):
    matching_data = 3       #클라이언트에서는 게임에 있는 패킷도 있지만 매칭서버는 매칭만 신경쓰면 됨
    matching_response = 4
    matching_decision = 5
    matching_retry = 6
    matching_complete = 7
    matching_reject = 8
    matching_cancel = 9


class MatchingPacketId(Enum):
    matching_request = 0
    matching_response = 1
    matching_catch = 2


class MatchingDecision(Enum):
    matching_accept = 0
    matching_reject = 1


# 클라이언트로 보내는 응답 ID
class MatchingResult(Enum):
    success = 0
    fail = 1


# 클라이언트로부터 받는 매칭 요청 패킷
class MatchingData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        matching_request = int.from_bytes(self.message[4:8], byteorder='big')

        packet = [packet_id, matching_request]

        return packet


class MatchingCancelData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        packet = [packet_id]
        return packet


# 클라이언트로 보낼 매칭 요청 응답 패킷
# 클라이언트로 보낼 패킷
class MatchingResponseData:
    def __init__(self, packet_id, matching_request, matching_result, my_info):
        self.data = [packet_id, matching_request, matching_result, my_info]
        #바이트로 변환해야함

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big') + \
                 self.data[2].to_bytes(4, byteorder='big') + \
                 self.data[3].to_bytes(4, byteorder='big')
        return packet


# 클라이언트로부터 매칭 거절 메시지 수신 패킷
class MatchingDecisionData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        decision = int.from_bytes(self.message[4:8], byteorder='big')
        my_info = int.from_bytes(self.message[8:12], byteorder='big')

        packet = [packet_id, decision, my_info]
        return packet


# 클라이언트로 보낼 재매치 메세지 패킷
class MatchingRetryData:
    def __init__(self, packet_id, matching_result):
        self.data = [packet_id, matching_result]

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big')
        return packet


class MatchingCompleteData:
    def __init__(self, packet_id, room_num, my_info):
        self.data = [packet_id, room_num, my_info]

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big') + \
                 self.data[2].to_bytes(4, byteorder='big')
        return packet


class MatchingRejectData:
    def __init__(self, packet_id, result):
        self.data = [packet_id, result]

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big')
        return packet
# 클라이언트로 보낼 매칭 성립 응답 패킷
