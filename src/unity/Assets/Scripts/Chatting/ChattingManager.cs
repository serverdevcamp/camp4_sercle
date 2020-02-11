/*
 * 로비에서의 채팅창을 관리하는 매니저
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class ChattingManager : MonoBehaviour
{
    // server ip, port
    private string address = "13.125.252.198";
    private int port = 3000;

    
    public UserInfo userInfo;
    public HTTPManager httpManager;
    
    public bool userInfoFlag = false;
    private TransportTCP socket;

    [Header("UI Holder")]
    public InputField inputField;
    public ScrollRect scrollbar;
    // 스크롤이 적용될 content. content의 child object로 전송받은 msg들이 생긴다.
    public Transform contentTr;
    // msg prefab
    public GameObject msgPrefab;

    void Start()
    {
        httpManager = new HTTPManager();
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        socket = GetComponent<TransportTCP>();
        Debug.Log("유저 : " + userInfo.userData.token);
        socket.Connect(address, port);
    }

    void Update()
    {
        if (userInfoFlag == false)
        {
            SendData(userInfo.userData.email);
            userInfoFlag = true;
        }

        ReceiveMessage();       //메세지 수신
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
    }

    private void SendData(string msg)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
        socket.Send(buffer, buffer.Length);
        Debug.Log("유저 정보 전송");
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
            string msg = System.Text.Encoding.Default.GetString(buffer);
            Debug.Log("Recv data : " + msg + msg);
            AddMessage(msg);
        }
    }

    void AddMessage(string message)
    {
        inputField.text = "";
        GameObject msg = Instantiate(msgPrefab);
        msg.transform.SetParent(contentTr);
        msg.GetComponent<Text>().text = message;
  
        msg.GetComponent<RectTransform>().localScale = Vector3.one;

        // 스크롤바를 항상 맨 아래로 지정.
       // scrollbar.verticalNormalizedPosition = 0f;
        scrollbar.verticalScrollbar.value = 0f;
    }
}
