/*
 * 매치메이킹 매니저 스크립트 입니다
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    // 매치메이킹이 완료되었을 때 화면에 등장할, 거절/수락 버튼을 가진 UI.
    public GameObject completeMatchMakingUI;
    // 매치메이킹이 완료되지 않았을 때 화면에 등장할 채팅창 등을 가진 UI. 
    public GameObject waitMatchMakingUI;
    // 매치메이킹이 완료되었는지 판단하는 변수
    public bool isMatchMakingCompleted;
    private MatchingNetworkManager networkManager;
    public static MatchingManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isMatchMakingCompleted = false;
        networkManager = GetComponent<MatchingNetworkManager>();
        networkManager.RegisterReceiveNotification(PacketId.MatchingResponse, OnReceiveMatchingResponsePacket);
    }

    public void testButton()
    {
        networkManager.ConnectIP();     //서버와 연결.
        Debug.Log("소켓 연결 Awd");
    }
    // Update is called once per frame
    void Update()
    {
        // 매치메이킹이 성공되어 true로 되었다면, 꺼져있던 매치메이킹 관련 UI가 화면에 등장.
        if(isMatchMakingCompleted == true)
        {
            // 꺼져있다면
            if (!completeMatchMakingUI.activeSelf)
            {
                // UI 켠다.
                completeMatchMakingUI.SetActive(true);
                // 다른 UI는 끈다.
                waitMatchMakingUI.SetActive(false);
            }
        }
        else
        {
            if (!waitMatchMakingUI.activeSelf)
            {
                waitMatchMakingUI.SetActive(true);
                completeMatchMakingUI.SetActive(false);
            }
        }
    }


    // 매칭 시작 버튼 클릭 함수
    public void MatchingRequest()
    {
        SendLocalMatchingRequest(1);
        Debug.Log("매칭 해주세요 버튼 클릭");
    }

    // 매치메이킹 수락
    public void AccpetMatchMakingResult()
    {
        Debug.Log("매칭 결과 수락 버튼 클릭");
        
        // 게임 씬으로 이동
    }
    // 매치메이킹 거절
    public void RefuseMatchMakingResult()
    {
        Debug.Log("매칭 결과 거절 버튼 클릭");
        // 초기 로비 화면으로 돌아감
        isMatchMakingCompleted = false;
    }


    //private void Matching

    //수락, 요청, 거절 -> 서버로 전송
    public void SendLocalMatchingRequest(int index)
    {
        MatchingData matchingData = new MatchingData();
        matchingData.request = MatchingPacketId.MatchingRequest;

        Debug.Log("매칭 데이터 : " + matchingData);
        MatchingPacket packet = new MatchingPacket(matchingData);

        networkManager.SendReliable<MatchingData>(packet);
    }

    public void SendLocalMatchingAccept()
    {
        MatchingData matchingData = new MatchingData();
        matchingData.request = MatchingPacketId.MatchingAccept;

        Debug.Log("매칭 데이터 : " + matchingData);
        MatchingPacket packet = new MatchingPacket(matchingData);

        networkManager.SendReliable<MatchingData>(packet);
    }

    public void SendLocalMatchingReject()
    {
        MatchingData matchingData = new MatchingData();
        matchingData.request = MatchingPacketId.MatchingReject;

        Debug.Log("매칭 데이터 : " + matchingData);
        MatchingPacket packet = new MatchingPacket(matchingData);

        networkManager.SendReliable<MatchingData>(packet);
    }
    //매칭 응답 패킷 받기
    //매칭관련된 모든 기능 여기 함수에 구현.
    public void OnReceiveMatchingResponsePacket(PacketId id, byte[] data)
    {
        //MatchingPacketId에 대한 처리해주기
        MatchingResponsePacket packet = new MatchingResponsePacket(data);
        MatchingResponseData packetData = packet.GetPacket();
        Debug.Log(packetData);

        if (packetData.request == MatchingPacketId.MatchingResponse)
        {
            if (packetData.result == MatchingResult.Success)
            {
                Debug.Log("매칭중~~~~~~~");
                //매칭 중인것을 클라이언트에 표시해야 함.
            }
            else
            {
                Debug.Log("매칭 실패");
            }
        }
        //매칭이 잡힘
        else if(packetData.request == MatchingPacketId.MatchingCatch)
        {
            if(packetData.result == MatchingResult.Success)
            {
                isMatchMakingCompleted = true;
                //수락 여부 버튼 띄우기.
            }
            else
            {
                Debug.Log("매칭 잡힘 실패");
            }
        }
        //매칭 수락
        else if(packetData.request == MatchingPacketId.MatchingAccept)
        {
            if(packetData.result == MatchingResult.Success)
            {
                Debug.Log("매칭 성공");
                //다음 작업
                //게임 서버로 바로 접속
            }
        }
        //매칭 거절
        else if(packetData.request == MatchingPacketId.MatchingReject)
        {
            if(packetData.result == MatchingResult.Success)
            {
                Debug.Log("매칭 거절");
            }
        }
        //요청 완료되면 화면을 매칭중으로 전환 아니면 오류 띄우기

    }
}
