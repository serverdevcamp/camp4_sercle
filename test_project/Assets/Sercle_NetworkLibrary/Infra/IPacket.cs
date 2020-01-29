/*
 * Send 함수가 패킷 데이터를 공통으로 다룰수있도록하는 인터페이스
 */ 
using System.Collections;
using System.IO;

public interface IPacket<T>
{
	// 패킷 id 얻기
	PacketId GetPacketId();

	//	패킷 데이터 얻기
	T GetPacket();

	// 바이너리 데이터 얻기
	byte[] GetData();
}