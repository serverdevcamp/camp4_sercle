/*
 * 로비에서의 채팅창을 관리하는 매니저
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChattingManager : MonoBehaviour
{
    // server ip, port
    private string address = "127.0.0.1";
    private int port = 3098;

    public InputField inputField;
    public UserInfo userInfo;
    public HTTPManager httpManager;
    public Text dialogue;
    public ScrollRect scrollbar;
    private TransportTCP socket = new TransportTCP();

    void Start()
    {
        httpManager = new HTTPManager();
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        Debug.Log("유저 : " + userInfo.userData.token);
        socket.Connect(address, port);

        dialogue.text = "";
    }

    void Update()
    {
        // 아무 입력이 없다면
        if(inputField.text.Length == 0)
        {
            return;
        }

        ReceiveMessage();

        // 입력 텍스트의 마지막 문자가 엔터라면
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string msg = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + inputField.text;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
            socket.Send(buffer, buffer.Length);

            dialogue.text += inputField.text;
            dialogue.text += "\n";
            inputField.text = "";
            scrollbar.verticalNormalizedPosition = 0f;
            
        }
        
    }

    private void ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        int recvSize = socket.Receive(ref buffer, buffer.Length);

        if(recvSize > 0)
        {
            string msg = System.Text.Encoding.UTF8.GetString(buffer);
            Debug.Log("Recv data : " + msg);
            AddMessage(msg);
        }
    }

    void AddMessage(string message)
    {
        dialogue.text += message;
        dialogue.text += "\n";
        inputField.text = "";
        scrollbar.verticalNormalizedPosition = 0f;
    }
    

}
