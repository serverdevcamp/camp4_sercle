/*  
 * 네트워크 관리자
 * 
 * 네트워크 연결/해제, 에러처리, 송/수신 담당.
 * 
 * FixedUpdate에서 수신
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;



public class NetworkManager : MonoBehaviour
{

    // 현재 이 단말이 네트워크에 연결되어 있는가.
    [SerializeField]
    private bool isNetConnected;

    // UDP
    private TransportUDP transportUDP;
    // TCP
    private TransportTCP transportTCP;

    // 수신 패킷 처리함수 델리게이트
    public delegate void RecvNotifier(PacketId id, byte[] data);

    // 수신 패킷 분배 해시 테이블
    private Dictionary<int, RecvNotifier> notifier = new Dictionary<int, RecvNotifier>();

    // Start is called before the first frame update
    void Start()
    {
        transportUDP = GetComponent<TransportUDP>();
        transportTCP = GetComponent<TransportTCP>();
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

    private void FixedUpdate()
    {
        
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
    // UDP 데이터 수신하는 함수
    private void ReceiveData()
    {
        byte[] data = new byte[1400];

        // 데이터 수신
        int recvSize = transportUDP.Receive(ref data, data.Length);
        //int recvSize = transportTCP.Receive(ref data, data.Length);

        if(recvSize <= 0)
        {
            // 데이터를 수신한게 없음.
            return;
        }

        string str = System.Text.Encoding.UTF8.GetString(data);
        if (str == "request connection.")
        {
            // 접속 요청 패킷이므로 아무 처리 않음
            return;
        }

        ReceivePacket(data);
    }

    // 수신된 데이터가 어느 데이터 구조에 적합한지 판단후 등록된 함수 호출
    public void ReceivePacket(byte[] data)
    {
        PacketHeader header = new PacketHeader();
        HeaderSerializer serializer = new HeaderSerializer();

        // 패킷 추출
        serializer.Deserialize(data, ref header);

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

    // 데이터를 서버로 전송(UDP)
    public int SendUnreliable<T>(IPacket<T> packet)
    {
        int sendSize = 0;
        
        if(transportUDP != null)
        {
            // 헤더 정보 생성
            PacketHeader header = new PacketHeader();
            HeaderSerializer serializer = new HeaderSerializer();

            // FIX THIS : 명시적 형변환 해줌. 소스코드와 다름
            header.packetId = (int)packet.GetPacketId();

            byte[] headerData = null;
            if(serializer.Serialize(header) == true)
            {
                headerData = serializer.GetSerializedData();
            }
            byte[] packetData = packet.GetData();

            byte[] data = new byte[headerData.Length + packetData.Length];

            int headerSize = Marshal.SizeOf(typeof(PacketHeader));
            Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
            Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);

            sendSize = transportUDP.Send(data, data.Length);
        }
        return sendSize;
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

            header.packetId = (int)packet.GetPacketId();

            byte[] headerData = null;
            if (serializer.Serialize(header) == true)
            {
                headerData = serializer.GetSerializedData();
            }

            byte[] packetData = packet.GetData();
            byte[] data = new byte[headerData.Length + packetData.Length];

            int headerSize = Marshal.SizeOf(typeof(PacketHeader));
            Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
            Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);

            string str = "Send reliable packet[" + header.packetId + "]";

            sendSize = transportTCP.Send(data, data.Length);
            //Debug.Log(data.Length + " 전송");
        }

        return sendSize;
    }

    // UDP 연결 요청
    // FIX THIS : 지금은 그저 Connect결과가 true면 성공으로 간주.(200122)
    public void ConnectIP()
    {
        if (!isNetConnected)
        {
            //Debug.Log("UDP 연결 버튼 클릭됨.");
            //SetNetConnectionStatus(transportUDP.Connect("127.0.0.1", 3098));
            //if (GetNetConnectionStatus())
            //    Time.timeScale = 1f;
            SetNetConnectionStatus(transportTCP.Connect("127.0.0.1", 3098));
            //transportTCP.Send(System.Text.Encoding.UTF8.GetBytes("PLEASEE"), 10);
            if (GetNetConnectionStatus())
            {
                Time.timeScale = 1f;
            }
        }
    }
}
