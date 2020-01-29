using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;
using System;
public class Chat : MonoBehaviour
{
    public InputField inputField;
    public Button button;

    //[TextArea]
    public Text history;

    // TCP
    private TransportTCP socket = new TransportTCP();

    // server ip, port
    private string address = "127.0.0.1";
    private int port = 3098;

    // 메시지 내역 저장
    public List<string> message = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        // 서버에 접속
        socket.Connect(address, port);
        // 통신 스레드 시작. Thread를 쓸 것이냐, Coroutine을 쓸 것이냐 선택해야함. 스레드는 일시정지 불가
        //socket.LaunceThread();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateChatting();
    }

    // 수신 엔드포인트
    private void UpdateChatting()
    {
        byte[] buffer = new byte[1024];

        int recvSize = socket.Receive(ref buffer, buffer.Length);
   
        if(recvSize > 0)
        {
            string msg = System.Text.Encoding.UTF8.GetString(buffer);
            Debug.Log("Recv data : " + msg);
            AddMessage(ref message, msg);
        }
    }

    // 대화 내역
    private void AddMessage(ref List<string> messages, string str)
    {
        // 10개 이상 넘어가면 First Input 삭제
        while(messages.Count >= 10)
        {
            messages.RemoveAt(0);
        }
        messages.Add(str);

        history.text = "";
        foreach(var item in messages)
            history.text += (item + '\n');
        
    }

    // send 버튼 클릭
    // 엔드포인트
    public void SendButton()
    {
        string msg = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + inputField.text;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
        socket.Send(buffer, buffer.Length);
        Debug.Log("Send data : " + msg);
        AddMessage(ref message, msg);
    }
}
