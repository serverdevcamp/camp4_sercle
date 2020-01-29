/*
 * 캐릭터패킷,
 * 채팅패킷,
 * 이동패킷,
 * 공격패킷 전부 작성해주어야함.
 * 
 * Packets
 */ 

using System.Collections;
using System.Collections.Generic;
using System.IO;

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
            ret &= Serialize(packet.index);
            ret &= Serialize(packet.num);
            ret &= Serialize(packet.dirX);
            ret &= Serialize(packet.dirY);
            ret &= Serialize(packet.dirZ);

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
            ret &= Deserialize(ref element.index);
            ret &= Deserialize(ref element.num);
            ret &= Deserialize(ref element.dirX);
            ret &= Deserialize(ref element.dirY);
            ret &= Deserialize(ref element.dirZ);

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
        return PacketId.CharacterData;
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
            if(GetDataSize() == 0)
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
