using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

public class MatchingNetworkManager : MonoBehaviour
{
    // 현재 이 단말이 네트워크에 연결되어 있는가.
    [SerializeField]
    private bool isNetConnected;
    // TCP
    private TransportTCP transportTCP;

    // 수신 패킷 처리함수 델리게이트
    public delegate void RecvNotifier(PacketId id, byte[] data);

    // 수신 패킷 분배 해시 테이블
    private Dictionary<int, RecvNotifier> notifier = new Dictionary<int, RecvNotifier>();

    // Start is called before the first frame update
    void Start()
    {
        transportTCP = GetComponent<TransportTCP>();
        transportTCP.Connect("10.99.13.48", 3098);
    }

    // Update is called once per frame
    void Update()
    {


        if (transportTCP.IsConnected())
        {
            ReceiveReliableData();
        }
        // ReceiveData();
    }

    // TCP로 데이터 수신하는 함수
    private void ReceiveReliableData()
    {
        byte[] packet = new byte[1400];
        while (transportTCP.Receive(ref packet, packet.Length) > 0)
        {
            // 수신패킷 분배
            ReceivePacket(packet);
        }
    }

    // 수신된 데이터가 어느 데이터 구조에 적합한지 판단후 등록된 함수 호출
    public void ReceivePacket(byte[] data)
    {
        PacketHeader header = new PacketHeader();
        HeaderSerializer serializer = new HeaderSerializer();

        // 패킷 추출
        // 맨앞자리만 추출
        serializer.Deserialize(data, ref header);

        //
        int packetId = (int)header.packetId;
        int headerSize = sizeof(int);
        byte[] packetData = new byte[data.Length - headerSize];
        Buffer.BlockCopy(data, headerSize, packetData, 0, packetData.Length);
        // 등록된 적절한 receive함수 호출
        notifier[packetId]((PacketId)packetId, packetData);
    }

    public bool GetNetConnectionStatus()
    {
        return isNetConnected;
    }


    public void SetNetConnectionStatus(bool value)
    {
        isNetConnected = value;
    }


    // 해시테이블에 패킷과 처리할 함수 등록
    public void RegisterReceiveNotification(PacketId id, RecvNotifier _notifier)
    {
        int index = (int)id;
        if (notifier.ContainsKey(index))
        {
            notifier.Remove(index);
        }

        notifier.Add(index, _notifier);
    }

    // 데이터를 서버로 전송(TCP)
    public int SendReliable<T>(IPacket<T> packet)
    {
        int sendSize = 0;

        if (transportTCP != null)
        {
            // 모듈에서 사용할 헤더 정보를 생성합니다.
            PacketHeader header = new PacketHeader();
            HeaderSerializer serializer = new HeaderSerializer();

            //packetid = skill, moving 등등   moving은 2
            header.packetId = (int)packet.GetPacketId();

            byte[] headerData = null;
            if (serializer.Serialize(header) == true)
            {
                headerData = serializer.GetSerializedData();
            }

            byte[] packetData = packet.GetData();   //움직임 정보 들은 데이터
            byte[] data = new byte[headerData.Length + packetData.Length];

            int headerSize = Marshal.SizeOf(typeof(PacketHeader));
            Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
            Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);

            string str = "Send reliable packet[" + header.packetId + "]";

            sendSize = transportTCP.Send(data, data.Length);
        }

        return sendSize;
    }

    // FIX THIS : 지금은 그저 Connect결과가 true면 성공으로 간주.(200122)
    public void ConnectIP()
    {
        if (!isNetConnected)
        {
            SetNetConnectionStatus(transportTCP.Connect("10.99.13.48", 3098));
            if (GetNetConnectionStatus())
            {
                Time.timeScale = 1f;
            }
        }
    }
}
