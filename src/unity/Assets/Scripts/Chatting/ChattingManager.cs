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
    private string address = "10.99.13.48";
    private int port = 3000;

    public InputField inputField;
    public UserInfo userInfo;
    public HTTPManager httpManager;
    public Text dialogue;
    public ScrollRect scrollbar;
    public bool userInfoFlag = false;
    private TransportTCP socket;

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
                StringBuilder sb = new StringBuilder("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + inputField.text, 100);
                string msg = inputField.text;
                SendData(msg);
            }
        }
    }

    private void LateUpdate()
    {
        // 테스트 코드.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            string[] str = new string[3];
            str[0] = "테스트1 ㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋㅋ";
            str[1] = "테스트2 ggggggggggggggggggㅎㅎㅎㅎㅎㅎㅎㅎㅎㅎㅎㅎㅎㅎ";
            str[2] = "테스트3 ㅋㅋㅋ";
            
            AddMessage(str[UnityEngine.Random.Range(0,3)]);
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
