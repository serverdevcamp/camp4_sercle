using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderSerializer : Serializer
{
	public bool Serialize(PacketHeader data)
	{
		// 기존 데이터를 클리어합니다.
		Clear();

		// 각 요소를 사례로 시리얼라이즈합니다.
		bool ret = true;
		ret &= Serialize((int)data.packetId);

		if (ret == false)
		{
			return false;
		}

		return true;
	}


	public bool Deserialize(byte[] data, ref PacketHeader serialized)
	{
		// 디시리얼라이즈할 데이터를 설정합니다.
		bool ret = SetDeserializedData(data);
		if (ret == false)
		{
			return false;
		}

		// 데이터의 요소별로 디시리얼라이즈합니다.
		int packetId = 0;
		ret &= Deserialize(ref packetId);
		// Fix this : 책이랑 다름
		serialized.packetId = (int)packetId;

		return ret;
	}
}
