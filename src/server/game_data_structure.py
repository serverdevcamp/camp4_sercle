from enum import Enum


class PacketId(Enum):
    skill_hit = 17      # 투사체에 히트시 사용되는 패킷 ID
    skill_data = 1      # 스킬 사용 요청시 사용되는 패킷 ID
    game_join = 10
    game_end = 12
    select_skill = 13   # 스킬 선택 정보
    game_finish = 14    # HQ 파괴 패킷 ID
    spawn_robots = 15   # 로봇(미니언) 스폰 패킷 ID
    game_start = 16     # 게임씬에 입장되었음을 의미하는 패킷 ID


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


# 게임 몬스터 생성 패킷
class SpawnRobotData:
    def __init__(self, packet_id, trash):
        self.data = [packet_id, trash]

    def serialize(self):
        packet = self.data[0].to_bytes(4, byteorder='big') + \
                 self.data[1].to_bytes(4, byteorder='big')
        return packet


# 클라이언트로부터 받은 스킬 선택 데이터
class SelectSkillData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        skill_idx = []
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        user_camp = int.from_bytes(self.message[4:8], byteorder='big')
        skill_idx.append(int.from_bytes(self.message[8:12], byteorder='big'))
        skill_idx.append(int.from_bytes(self.message[12:16], byteorder='big'))
        skill_idx.append(int.from_bytes(self.message[16:20], byteorder='big'))
        packet = [packet_id, user_camp, skill_idx]
        return packet


# HQ 파괴되었다는 데이터 중계
class GameFinishData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        winner_camp = int.from_bytes(self.message[4:8], byteorder='big')
        packet = [packet_id, winner_camp]
        return packet


# 스킬 히트 데이터 중계
class SkillHitData:
    def __init__(self, message):
        self.message = message

    def deserialize(self):
        packet_id = int.from_bytes(self.message[0:4], byteorder='big')
        index = int.from_bytes(self.message[4:8], byteorder='big')
        status_type = int.from_bytes(self.message[8:12], byteorder='big')
        cc_type = int.from_bytes(self.message[12:16], byteorder='big')
        amount = int.from_bytes(self.message[16:20], byteorder='big')
        duration = int.from_bytes(self.message[20:24], byteorder='big')
        packet = [packet_id, index, status_type, cc_type, amount, duration]
        return packet


# 게임 시작(클라이언트 단에서 송신함) 데이터 중계
class GameStartData:
    def __init__(self, message):
        self.message = message


