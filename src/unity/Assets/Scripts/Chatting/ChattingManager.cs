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
    private int port = 3000;

    public InputField inputField;
    public UserInfo userInfo;
    public HTTPManager httpManager;
    public Text dialogue;
    public ScrollRect scrollbar;
    public bool userInfoFlag = false;
    private TransportTCP socket;

    void Start()
    {
        httpManager = new HTTPManager();
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        socket = GetComponent<TransportTCP>();
        Debug.Log("유저 : " + userInfo.userData.token);
        socket.Connect(address, port);

        dialogue.text = "";
    }

    void Update()
    {
        if (userInfoFlag == false)
        {
            SendData(userInfo.userData.email);
            userInfoFlag = true;
        }

        ReceiveMessage();       //메세지 수신

        // 입력 텍스트의 마지막 문자가 엔터라면
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 아무 입력이 없다면
            if (inputField.text.Length == 0)
            {
                Debug.Log("문자가 없음");
                return;
            }
            if (CheckUser(userInfo.userData.email, userInfo.userData.token))
            {
                Debug.Log("zzzzz");
                string msg = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + inputField.text;
                SendData(msg);
            }
        }
    }

    private void SendData(string msg)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
        socket.Send(buffer, buffer.Length);
        Debug.Log("유저 정보 전송");
    }
    private Boolean CheckUser(string email, string token)
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
            string endcode = Convert.ToBase64String(buffer);
            byte[] decode = Convert.FromBase64String(endcode);

            string msg = System.Text.Encoding.UTF8.GetString(decode);
            Debug.Log("Recv data : " + msg);
            AddMessage(msg);
        }
    }

    void AddMessage(string message)
    {
        ///string a =  message.ToString() + "1";
       // Debug.Log("메세지1 : " + a);
        //Debug.Log("메세지2 : " + message);
        dialogue.text += "a";

        inputField.text = "";
        scrollbar.verticalNormalizedPosition = 0f;
    }
    

}
