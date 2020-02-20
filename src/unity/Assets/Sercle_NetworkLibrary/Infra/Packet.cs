/*
 * 캐릭터패킷,
 * 채팅패킷,
 * 이동패킷,
 * 매칭패킷,
 * 공격패킷 전부 작성해주어야함.
 * 
 * Packets
 */ 

using System.Collections;
using System.Collections.Generic;
using System.IO;

// SyncData 전송용 Packet
public class SyncPacket : IPacket<SyncData>
{
    SyncData packet;

    public SyncPacket(SyncData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public SyncPacket(byte[] data)
    {
        SyncSerializer serializer = new SyncSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SyncData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public SyncData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        SyncSerializer serializer = new SyncSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class SyncSerializer : Serializer
    {
        public bool Serialize(SyncData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.sendTime);

            return ret;
        }

        public bool Deserialize(ref SyncData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.sendTime);

            return ret;
        }
    }
}

public class GameJoinPacket : IPacket<GameJoinData>
{
    GameJoinData packet;

    public GameJoinPacket(GameJoinData data)
    {
        packet = data;
    }

    public GameJoinPacket(byte[] data)
    {
        GameJoinSerializer serializer = new GameJoinSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.GameServerJoin;
    }

    public GameJoinData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        GameJoinSerializer serializer = new GameJoinSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class GameJoinSerializer : Serializer
    {
        public bool Serialize(GameJoinData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.id);
            ret &= Serialize(packet.campNum);
            ret &= Serialize(packet.roomNum);
            return ret;
        }
        public bool Deserialize(ref GameJoinData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;
            ret &= Deserialize(ref element.id);
            ret &= Deserialize(ref element.campNum);
            ret &= Deserialize(ref element.roomNum);
            return ret;
        }
    }
}
public class GameEndPacket : IPacket<GameEndData>
{
    GameEndData packet;
    public GameEndPacket(GameEndData data)
    {
        packet = data;
    }
    public GameEndPacket(byte[] data)
    {
        GameEndSerializer serializer = new GameEndSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.GameServerEnd;
    }
    public GameEndData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        GameEndSerializer serializer = new GameEndSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class GameEndSerializer : Serializer
    {
        public bool Serialize(GameEndData packet)
        {
            bool ret = true;
            int request = (int)packet.request;
            ret &= Serialize(request);
            return ret;
        }
        public bool Deserialize(ref GameEndData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;
            int request = 0;
            ret &= Deserialize(ref request);
            element.request = (GamePacketId)request;
            return ret;
        }
    }
}
// Moving Data 전송용 Packet
public class MovingPacket : IPacket<MovingData>
{
    MovingData packet;

    public MovingPacket(MovingData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public MovingPacket(byte[] data)
    {
        MovingSerializer serializer = new MovingSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.MovingData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public MovingData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        MovingSerializer serializer = new MovingSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class MovingSerializer : Serializer
    {
        public bool Serialize(MovingData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.index);
            ret &= Serialize(packet.destX);
            ret &= Serialize(packet.destY);
            ret &= Serialize(packet.destZ);

            return ret;
        }

        public bool Deserialize(ref MovingData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.index);
            ret &= Deserialize(ref element.destX);
            ret &= Deserialize(ref element.destY);
            ret &= Deserialize(ref element.destZ);

            return ret;
        }
    }
}

//클라이언트가 서버로 전송할 패킷
public class MatchingPacket : IPacket<MatchingData>
{
    MatchingData packet;

    public MatchingPacket(MatchingData data)
    {
        packet = data;
    }

    public MatchingPacket(byte[] data)
    {
        MatchingSerializer serializer = new MatchingSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.MatchingData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public MatchingData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        MatchingSerializer serializer = new MatchingSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class MatchingSerializer : Serializer
    {
        public bool Serialize(MatchingData packet)
        {
            bool ret = true;

            int request = (int)packet.request;
            ret &= Serialize(request);
            return ret;
        }

        public bool Deserialize(ref MatchingData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;

            int request = 0;
            ret &= Deserialize(ref request);
            element.request = (MatchingPacketId)request;

            return ret;
        }
    }
}

public class MatchingCancelPacket : IPacket<MatchingCancelData>
{
    MatchingCancelData packet;
    public MatchingCancelPacket(MatchingCancelData data)
    {
        packet = data;
    }

    public MatchingCancelPacket(byte[] data)
    {
        MatchingCancelSerializer serializer = new MatchingCancelSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.MatchingCancel;
    }
    public MatchingCancelData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        MatchingCancelSerializer serializer = new MatchingCancelSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class MatchingCancelSerializer : Serializer
    {
        public bool Serialize(MatchingCancelData packet)
        {
            bool ret = true;

            ret &= Serialize(packet.myInfo);
            return ret;
        }
        public bool Deserialize(ref MatchingCancelData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;

            ret &= Deserialize(ref element.myInfo);
            return ret;
        }
    }
}
//서버가 클라이언트로 전송한 패킷
public class MatchingResponsePacket : IPacket<MatchingResponseData>
{
    MatchingResponseData packet;

    public MatchingResponsePacket(MatchingResponseData data)
    {
        packet = data;
    }
    public MatchingResponsePacket(byte[] data)
    {
        MatchingResponseSerializer serializer = new MatchingResponseSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.MatchingResponse;
    }

    public MatchingResponseData GetPacket()
    {
        return packet;
    }

    public byte[] GetData()
    {
        MatchingResponseSerializer serializer = new MatchingResponseSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class MatchingResponseSerializer : Serializer
    {
        public bool Serialize(MatchingResponseData packet)
        {
            bool ret = true;

            int request = (int)packet.request;
            ret &= Serialize(request);
            int result = (int)packet.result;
            ret &= Serialize(result);
            ret &= Serialize(packet.myInfo);

            return ret;
        }

        public bool Deserialize(ref MatchingResponseData element)
        {
            if(GetDataSize() == 0) {
                // 데이터가 설정되어 있지 않습니다.
                return false;
            }

            bool ret = true;

            int request = 0;
            ret &= Deserialize(ref request);
            element.request = (MatchingPacketId)request;

            int result = 0;
            ret &= Deserialize(ref result);
            element.result = (MatchingResult)result;

            ret &= Deserialize(ref element.myInfo);
            return ret;
        }
    }
}

public class MatchingDecisionPacket: IPacket<MatchingDecisionData>
{
    MatchingDecisionData packet;
    public MatchingDecisionPacket(MatchingDecisionData data)
    {
        packet = data;
    }
    public MatchingDecisionPacket(byte[] data)
    {
        MatchingDecisionSerializer serializer = new MatchingDecisionSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.MatchingDecision;
    }
    public MatchingDecisionData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        MatchingDecisionSerializer serializer = new MatchingDecisionSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class MatchingDecisionSerializer : Serializer
    {
        public bool Serialize(MatchingDecisionData packet)
        {
            bool ret = true;

            int decision = (int)packet.decision;
            ret &= Serialize(decision);
            ret &= Serialize(packet.myinfo);

            return ret;
        }
        public bool Deserialize(ref MatchingDecisionData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;

            int decision = 0;
            ret &= Deserialize(ref decision);
            element.decision = (MatchingDecision)decision;

            ret &= Deserialize(ref element.myinfo);

            return ret;
        }

    }
}

public class MatchingCompletePacket: IPacket<MatchingCompleteData>
{
    MatchingCompleteData packet;
    public MatchingCompletePacket(MatchingCompleteData data)
    {
        packet = data;
    }
    public MatchingCompletePacket(byte[] data)
    {
        MatchingCompleteSerializer serializer = new MatchingCompleteSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.MatchingComplete;
    }
    public MatchingCompleteData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        MatchingCompleteSerializer serializer = new MatchingCompleteSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class MatchingCompleteSerializer : Serializer
    {
        public bool Serialize(MatchingCompleteData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.roomId);
            ret &= Serialize(packet.myInfo);
            ret &= Serialize(packet.playerCamp);
            return ret;
        }
        public bool Deserialize(ref MatchingCompleteData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;
            ret &= Deserialize(ref element.roomId);
            ret &= Deserialize(ref element.myInfo);
            ret &= Deserialize(ref element.playerCamp);
            return ret;
        }
    }
}
public class MatchingRetryPacket : IPacket<MatchingRetryData>
{
    MatchingRetryData packet;
    public MatchingRetryPacket(MatchingRetryData data)
    {
        packet = data;
    }
    public MatchingRetryPacket(byte[] data)
    {
        MatchingRetrySerializer serializer = new MatchingRetrySerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.MatchingRetry;
    }
    public MatchingRetryData GetPacket()
    {
        return packet;
    }
    public  byte[] GetData()
    {
        MatchingRetrySerializer serializer = new MatchingRetrySerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class MatchingRetrySerializer : Serializer
    {
        public bool Serialize(MatchingRetryData packet)
        {
            bool ret = true;
            int result = (int)packet.result;
            ret &= Serialize(result);
            return ret;
        }

        public bool Deserialize(ref MatchingRetryData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;
            int result = 0;
            ret &= Deserialize(ref result);
            element.result = (MatchingResult)result;
            return ret;
        }
    }
}

public class MatchingRejectPacket : IPacket<MatchingRejectData>
{
    MatchingRejectData packet;
    public MatchingRejectPacket(MatchingRejectData data)
    {
        packet = data;
    }
    public MatchingRejectPacket(byte[] data)
    {
        MatchingRejectSerializer serializer = new MatchingRejectSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }
    public PacketId GetPacketId()
    {
        return PacketId.MatchingReject;
    }
    public MatchingRejectData GetPacket()
    {
        return packet;
    }
    public byte[] GetData()
    {
        MatchingRejectSerializer serializer = new MatchingRejectSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }
    class MatchingRejectSerializer : Serializer
    {
        public bool Serialize(MatchingRejectData packet)
        {
            bool ret = true;
            int result = (int)packet.result;
            ret &= Serialize(result);
            return ret;
        }
        public bool Deserialize(ref MatchingRejectData element)
        {
            if(GetDataSize() == 0)
            {
                return false;
            }
            bool ret = true;
            int result = 0;
            ret &= Deserialize(ref result);
            element.result = (MatchingResult)result;
            return ret;
        }
    }

}

// Skill Data 전송용 Packet
public class SkillPacket : IPacket<SkillData>
{
    SkillData packet;

    public SkillPacket(SkillData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public SkillPacket(byte[] data)
    {
        SkillSerializer serializer = new SkillSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SkillData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public SkillData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        SkillSerializer serializer = new SkillSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class SkillSerializer : Serializer
    {
        public bool Serialize(SkillData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.campNumber);
            ret &= Serialize(packet.isRobot);
            ret &= Serialize(packet.index);
            ret &= Serialize(packet.dirX);
            ret &= Serialize(packet.dirY);
            ret &= Serialize(packet.dirZ);
            ret &= Serialize(packet.posX);
            ret &= Serialize(packet.posY);
            ret &= Serialize(packet.posZ);

            return ret;
        }

        public bool Deserialize(ref SkillData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.campNumber);
            ret &= Deserialize(ref element.isRobot);
            ret &= Deserialize(ref element.index);
            ret &= Deserialize(ref element.dirX);
            ret &= Deserialize(ref element.dirY);
            ret &= Deserialize(ref element.dirZ);
            ret &= Deserialize(ref element.posX);
            ret &= Deserialize(ref element.posY);
            ret &= Deserialize(ref element.posZ);

            return ret;
        }
    }
}


// Character Data 전송용 Packet
public class CharacterPacket : IPacket<CharacterData>
{

    CharacterData packet;

    // 패킷 데이터를 직렬화하는 생성자
    public CharacterPacket(CharacterData data)
    {
        packet = data;
    }

    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public CharacterPacket(byte[] data)
    {
        CharacterSerializer serializer = new CharacterSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SkillData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public CharacterData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        CharacterSerializer serializer = new CharacterSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class CharacterSerializer : Serializer
    {
        public bool Serialize(CharacterData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.playerIndex);
            ret &= Serialize(packet.mhp);
            ret &= Serialize(packet.chp);
            ret &= Serialize(packet.spd);
            ret &= Serialize(packet.atk);
            ret &= Serialize(packet.def);
            ret &= Serialize(packet.crt);
            ret &= Serialize(packet.ddg);
            ret &= Serialize(packet.cc);

            return ret;
        }

        public bool Deserialize(ref CharacterData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.playerIndex);
            ret &= Deserialize(ref element.mhp);
            ret &= Deserialize(ref element.chp);
            ret &= Deserialize(ref element.spd);
            ret &= Deserialize(ref element.atk);
            ret &= Deserialize(ref element.def);
            ret &= Deserialize(ref element.crt);
            ret &= Deserialize(ref element.ddg);
            ret &= Deserialize(ref element.cc);

            return ret;
        }
    }
}
// 스킬 선택 씬에서 결정한 스킬 번호 패킷
public class SelectedSkillPacket : IPacket<SelectedSkillData>
{
    SelectedSkillData packet;

    public SelectedSkillPacket(SelectedSkillData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public SelectedSkillPacket(byte[] data)
    {
        SelectedSkillSerializer serializer = new SelectedSkillSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SelectedSkillData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public SelectedSkillData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        SelectedSkillSerializer serializer = new SelectedSkillSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class SelectedSkillSerializer : Serializer
    {
        public bool Serialize(SelectedSkillData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.userCamp);
            ret &= Serialize(packet.skillIndex[0]);
            ret &= Serialize(packet.skillIndex[1]);
            ret &= Serialize(packet.skillIndex[2]);

            return ret;
        }

        public bool Deserialize(ref SelectedSkillData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.userCamp);
            element.skillIndex = new int[3];
            ret &= Deserialize(ref element.skillIndex[0]);
            ret &= Deserialize(ref element.skillIndex[1]);
            ret &= Deserialize(ref element.skillIndex[2]);

            return ret;
        }
    }
}

// HQ가 파괴되었다는 패킷 전송.
public class GameFinishPacket : IPacket<GameFinishData>
{
    GameFinishData packet;

    public GameFinishPacket(GameFinishData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public GameFinishPacket(byte[] data)
    {
        GameFinishSerializer serializer = new GameFinishSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.GameFinish;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public GameFinishData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        GameFinishSerializer serializer = new GameFinishSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class GameFinishSerializer : Serializer
    {
        public bool Serialize(GameFinishData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.winnerCamp);

            return ret;
        }

        public bool Deserialize(ref GameFinishData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.winnerCamp);

            return ret;
        }
    }
}

// 스킬 투사체에 피격 되었다는 패킷 전송.
public class SkillHitPacket : IPacket<SkillHitData>
{
    SkillHitData packet;

    public SkillHitPacket(SkillHitData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public SkillHitPacket(byte[] data)
    {
        SkillHitSerializer serializer = new SkillHitSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SkillHitData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public SkillHitData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        SkillHitSerializer serializer = new SkillHitSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class SkillHitSerializer : Serializer
    {
        public bool Serialize(SkillHitData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.campNumber);
            ret &= Serialize(packet.index);
            ret &= Serialize(packet.statusType);
            ret &= Serialize(packet.ccType);
            ret &= Serialize(packet.amount);
            ret &= Serialize(packet.duration);
            ret &= Serialize(packet.chp);
            ret &= Serialize(packet.trash1);
            ret &= Serialize(packet.trash2);
            return ret;
        }

        public bool Deserialize(ref SkillHitData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.campNumber);
            ret &= Deserialize(ref element.index);
            ret &= Deserialize(ref element.statusType);
            ret &= Deserialize(ref element.ccType);
            ret &= Deserialize(ref element.amount);
            ret &= Deserialize(ref element.duration);
            ret &= Deserialize(ref element.chp);
            ret &= Deserialize(ref element.trash1);
            ret &= Deserialize(ref element.trash2);

            return ret;
        }
    }
}

public class GameStartPacket : IPacket<GameStartData>
{
    GameStartData packet;

    public GameStartPacket(GameStartData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public GameStartPacket(byte[] data)
    {
        GameStartSerializer serializer = new GameStartSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.GameStart;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public GameStartData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        GameStartSerializer serializer = new GameStartSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class GameStartSerializer : Serializer
    {
        public bool Serialize(GameStartData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.campNumber);

            return ret;
        }

        public bool Deserialize(ref GameStartData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.campNumber);

            return ret;
        }
    }
}

public class SpawnRobotPacket : IPacket<SpawnRobotData>
{
    SpawnRobotData packet;

    public SpawnRobotPacket(SpawnRobotData data)
    {
        packet = data;
    }
    // 바이너리 데이터를 패킷 데이터로 역직렬화하는 생성자
    public SpawnRobotPacket(byte[] data)
    {
        SpawnRobotSerializer serializer = new SpawnRobotSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref packet);
    }

    public PacketId GetPacketId()
    {
        return PacketId.SpawnRobotsData;
    }

    // 게임에서 사용할 패킷 데이터 획득
    public SpawnRobotData GetPacket()
    {
        return packet;
    }

    // 송신용 byte[] 형 데이터 획득
    public byte[] GetData()
    {
        SpawnRobotSerializer serializer = new SpawnRobotSerializer();
        serializer.Serialize(packet);
        return serializer.GetSerializedData();
    }

    class SpawnRobotSerializer : Serializer
    {
        public bool Serialize(SpawnRobotData packet)
        {
            bool ret = true;
            ret &= Serialize(packet.trash);
            return ret;
        }

        public bool Deserialize(ref SpawnRobotData element)
        {
            // 데이터가 정의되어있지 않다면
            if (GetDataSize() == 0)
            {
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.trash);
            return ret;
        }
    }
}

