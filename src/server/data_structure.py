from enum import Enum


class MatchingPacketId(Enum):
    matching_request = 0
    matching_accept = 1
    matching_reject = 2


class MatchingData:
    def __init__(self, matching_packet_id, index, room_num):
        self.matching_packet_id = matching_packet_id.value
        self.index = index
        self.room_num = room_num


#print(0 == MatchingPacketId.matching_request.value)