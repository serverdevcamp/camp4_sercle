using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    public GameObject MatchingResponseWaitUI;
    // 매치메이킹이 완료되었을 때 화면에 등장할, 거절/수락 버튼을 가진 UI.
    public GameObject completeMatchMakingUI;
    // 매치메이킹이 완료되지 않았을 때 화면에 등장할 채팅창 등을 가진 UI. 
    public GameObject waitMatchMakingUI;
    // 매치메이킹이 완료되었는지 판단하는 변수
    public bool isMatchMakingCompleted;
    public bool isMatchingResponseWait;
    private MatchingNetworkManager networkManager;
    public static MatchingManager instance;
    public int myInfo;
    public int opponentInfo;
    public int roomNum;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isMatchMakingCompleted = false;
        isMatchingResponseWait = false;
        networkManager = GetComponent<MatchingNetworkManager>();
        networkManager.RegisterReceiveNotification(PacketId.MatchingResponse,
            OnReceiveMatchingResponsePacket);
        networkManager.RegisterReceiveNotification(PacketId.MatchingRetry,
            OnReceiveMatchingRetryPacket);
        networkManager.RegisterReceiveNotification(PacketId.MatchingReject,
            OnReceiveMatchingRejectPacket);
    }

    // Update is called once per frame
    void Update()
    {
        if(isMatchingResponseWait == true)
        {
            if (!MatchingResponseWaitUI.activeSelf)
            {
                MatchingResponseWaitUI.SetActive(true);
                waitMatchMakingUI.SetActive(false);
                completeMatchMakingUI.SetActive(false);
            }
        }
        else
        {
            MatchingResponseWaitUI.SetActive(false);
        }
        
        if(isMatchMakingCompleted == true)
        {
            // 꺼져있다면
            if (!completeMatchMakingUI.activeSelf)
            {
                completeMatchMakingUI.SetActive(true);
                waitMatchMakingUI.SetActive(false);
                MatchingResponseWaitUI.SetActive(false);
            }
        }
        else
        {
            completeMatchMakingUI.SetActive(false);
        }

        if(isMatchingResponseWait == false && isMatchMakingCompleted == false)
        {
            if (!waitMatchMakingUI)
            {
                waitMatchMakingUI.SetActive(true);
            }
        }
    }

    // 매칭 시작 버튼 클릭 함수
    public void MatchingRequest()
    {
        SendLocalMatchingRequest();
        Debug.Log("매칭 해주세요 버튼 클릭");
    }

    // 매치메이킹 수락
    public void AccpetMatchMakingResult()
    {
        Debug.Log("매칭 결과 수락 버튼 클릭");
        SendLocalMatchingAccept();

        // 상대방 매칭 대기중 메시지 띄우기
        // 게임 씬으로 이동
    }

    // 매치메이킹 거절
    public void RefuseMatchMakingResult()
    {
        Debug.Log("매칭 결과 거절 버튼 클릭");
        // 초기 로비 화면으로 돌아감
        SendLocalMatchingReject();
    }

    //매칭 요청
    public void SendLocalMatchingRequest()
    {
        MatchingData matchingData = new MatchingData();
        matchingData.request = MatchingPacketId.MatchingRequest;
        Debug.Log("매칭 데이터 : " + matchingData);
        MatchingPacket packet = new MatchingPacket(matchingData);
        networkManager.SendReliable<MatchingData>(packet);
    }

    //매칭 수락
    public void SendLocalMatchingAccept()
    {
        MatchingDecisionData matchingDecisionData = new MatchingDecisionData();
        matchingDecisionData.decision = MatchingDecision.Accept;
        matchingDecisionData.myinfo = myInfo;
        MatchingDecisionPacket packet = new MatchingDecisionPacket(matchingDecisionData);
        networkManager.SendReliable<MatchingDecisionData>(packet);
        isMatchingResponseWait = true;
        isMatchMakingCompleted = false;
    }

    //매칭 거절
    public void SendLocalMatchingReject()
    {
        MatchingDecisionData matchingDecisionData = new MatchingDecisionData();
        matchingDecisionData.decision = MatchingDecision.Reject;
        matchingDecisionData.myinfo = myInfo;
        MatchingDecisionPacket packet = new MatchingDecisionPacket(matchingDecisionData);
        networkManager.SendReliable<MatchingDecisionData>(packet);
        isMatchingResponseWait = true;
        isMatchMakingCompleted = false;
    }

    //매칭 응답 패킷 받기
    //매칭 중, 매칭 잡힘 처리
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
                //매칭 중인것을 클라이언트에 표시해야 함
            }
            else
            {
                Debug.Log("매칭 실패");
            }
        }
        //매칭이 잡힘 
        else if(packetData.request == MatchingPacketId.MatchingCatch)
        {
            myInfo = -1;

            if(packetData.result == MatchingResult.Success)
            {
                myInfo = packetData.myInfo;
                isMatchMakingCompleted = true;
                //내 정보 상대 정보 저장.
                //수락 여부 버튼 띄우기.
            }
            else
            {
                Debug.Log("매칭 잡힘 실패");
            }
        }  
    }

    //상대방이 매칭을 거절했을시 재매치
    public void OnReceiveMatchingRetryPacket(PacketId id, byte[] data)
    {
        Debug.Log("retryPacket");
        MatchingRetryPacket packet = new MatchingRetryPacket(data);
        MatchingRetryData packetData = packet.GetPacket();

        //
        if(packetData.result == MatchingResult.Success)
        {
            Debug.Log("재매칭 중");
            //재매칭중 띄우기.
            isMatchMakingCompleted = false;
            //
        }
    }

    public void OnReceiveMatchingRejectPacket(PacketId id, byte[] data)
    {
        Debug.Log("rejectPacket");
        MatchingRejectPacket packet = new MatchingRejectPacket(data);
        MatchingRejectData packetData = packet.GetPacket();

        if(packetData.result == MatchingResult.Success)
        {
            Debug.Log("매칭 거절 됌");
            isMatchingResponseWait = false;
        }
    }
    //둘 다 매칭을 수락했을 때의 메세지
    public void OnReceiveMatchingCompletePacket(PacketId id, byte[] data)
    {
        MatchingCompletePacket packet = new MatchingCompletePacket(data);
        MatchingCompleteData packetData = packet.GetPacket();
        Debug.Log("둘 다 매칭 수락 게임을 시작합니다.");
    }
    //매칭 수락후 서버에서 게임 접속 메세지
    public void OnReceiveGameStart(PacketId id, byte[] data)
    {

    }
}
