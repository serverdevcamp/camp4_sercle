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

    public void SendLocalMatchingRequest(int index)
    {
        MatchingData matchingData = new MatchingData();
        matchingData.index = index;
        matchingData.roomNum = -1;
        matchingData.matchingPacketId = MatchingPacketId.MatchingRequest;

        MatchingPacket packet = new MatchingPacket(matchingData);

        networkManager.SendReliable<MatchingData>(packet);
    }

    //매칭 응답 패킷 받기
    public void OnReceiveMatchingResponsePacket(PacketId id, byte[] data)
    {
        MatchingResponsePacket packet = new MatchingResponsePacket(data);
        MatchingResponseData packetData = packet.GetPacket();
        Debug.Log(packetData);

        if(packetData.result == MatchingResult.Success)
        {
            Debug.Log("매칭중~~~~~~~");
        }
        //요청 완료되면 화면을 매칭중으로 전환 아니면 오류 띄우기

    }
}
