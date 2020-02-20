/*
 * 로비에서의 채팅창을 관리하는 매니저
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using DG.Tweening;
using System.Runtime.InteropServices;

public class ChattingManager : MonoBehaviour
{
    // server ip, port
    private string address = Info.IP;
    private int port = 3000;

    
    public UserInfo userInfo;
    public HTTPManager httpManager;
    
    public bool userInfoFlag = false;
    private TransportTCP socket;

    [Header("UI Holder")]
    public InputField inputField;
    public Scrollbar scrollbar;
    // 스크롤이 적용될 content. content의 child object로 전송받은 msg들이 생긴다.
    public Transform contentTr;
    // msg prefab
    public GameObject msgPrefab;

    private Serializer.Endianness m_endianness;
    void Start()
    {
        httpManager = new HTTPManager();
        userInfo = GameObject.Find("DataObject").GetComponent<UserInfo>();
        socket = GetComponent<TransportTCP>();
        Debug.Log("유저 : " + userInfo.userData.token);
        socket.Connect(address, port);

        // 엔디언을 판정합니다.
        int val = 1;
        byte[] conv = BitConverter.GetBytes(val);
        m_endianness = (conv[0] == 1) ? Serializer.Endianness.LittleEndian : Serializer.Endianness.BigEndian;

        // StartCoroutine(ChatTest());
    }

    void Update()
    {
        if (userInfoFlag == false)
        {
            SendData(userInfo.userData.username);
            userInfoFlag = true;
        }

        ReceiveMessage();       //메세지 수신
        OnTypeChat();   // 타이핑 되고 있는지 판단 후 사운드 재생
    }
    IEnumerator ChatTest()
    {
        int cnt = 0;
        yield return new WaitForSeconds(5f);
        while (cnt < 200)
        {
            string str = (cnt++ + 1).ToString();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);

            socket.Send(buffer, buffer.Length);
            yield return new WaitForFixedUpdate();
        }
    }
    public void OnEndEdit()
    {
        // 아무 입력이 없다면
        if (inputField.text.Length == 0)
        {
            Debug.Log("문자가 없음");
            return;
        }
        if (CheckUser(userInfo.userData.email, userInfo.userData.token))
        {
            StringBuilder sb = new StringBuilder("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + inputField.text, 100);
            string msg = inputField.text;
            SendData(msg);
        }

        inputField.text = "";
    }

    private void SendData(string msg)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);

        PacketHeader header = new PacketHeader();
        HeaderSerializer serializer = new HeaderSerializer();

        header.packetSize = sizeof(int) + buffer.Length;
        header.packetId = (int)PacketId.ChatData;

        byte[] headerData = null;
        if(serializer.Serialize(header) == true)
        {
            headerData = serializer.GetSerializedData();
        }

        // 내 컴퓨터가 리틀엔디언이면 문자열 역전해서 보낸다.
        if (m_endianness == Serializer.Endianness.LittleEndian)
        {
            Array.Reverse(buffer);
        }

        int headerSize = Marshal.SizeOf(typeof(PacketHeader));

        byte[] data = new byte[headerData.Length + buffer.Length];

        Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
        Buffer.BlockCopy(buffer, 0, data, headerSize, buffer.Length);

        int sendSize = socket.Send(data, data.Length);

        Debug.Log("유저 정보 전송완료," + " 보낸 양 : " + sendSize + " 헤더사이즈 : " + headerSize + " 채팅내용 길이 : " + buffer.Length);
    }

    private bool CheckUser(string email, string token)
    {
        if (httpManager.UserCacheReq(email) == token)
            return true;
        return false;
    }

    private void OnDestroy()
    {
        Debug.Log("socket Disconnect");
        socket.Disconnect();
    }

    private void ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        int recvSize = socket.Receive(ref buffer, buffer.Length);
 
        if(recvSize > 0)
        {
            PacketHeader header = new PacketHeader();
            HeaderSerializer serializer = new HeaderSerializer();

            // 맨앞자리만 추출
            serializer.Deserialize(buffer, ref header);

            int packetId = (int)header.packetId;
            int headerSize = sizeof(int);

            if (packetId != (int)PacketId.ChatData)
            {
                Debug.LogError("패킷 아이디가 채팅이 아닙니다.");
            }

            byte[] packetData = new byte[buffer.Length - headerSize];
            Buffer.BlockCopy(buffer, headerSize, packetData, 0, packetData.Length);
            
            if (m_endianness == Serializer.Endianness.LittleEndian)
            {
                Array.Reverse(packetData);
            }

            string msg = System.Text.Encoding.Default.GetString(packetData);
            Debug.Log("Recv data : " + msg);

            AddMessage(msg);
        }
    }

    void AddMessage(string message)
    {
        GameObject msg = Instantiate(msgPrefab);
        msg.transform.SetParent(contentTr);
        msg.GetComponent<Text>().text = message;
  
        msg.GetComponent<RectTransform>().localScale = Vector3.one;

        // 스크롤바를 항상 맨 아래로 지정.
        DOTween.To(() => scrollbar.value, x => scrollbar.value = x, 0, 1f);
        // 새로운 메시지가 도착했을 경우 도착 사운드 재생
        SoundManager.instance.PlaySound("Lobby_NewMessage", 0.5f);
    }

    // InputField에 글자를 타이핑 하고 있을 경우 타이핑 사운드 재생
    private void OnTypeChat()
    {
        if (inputField.isFocused)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SoundManager.instance.PlaySound("ButtonClick");
                }
                else
                {
                    SoundManager.instance.PlaySound("TextType");
                }
            }
        }
    }
}
