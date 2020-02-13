using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchingManager : MonoBehaviour
{
    [Header("Info Holder")]
    private MatchingNetworkManager networkManager;
    public static MatchingManager instance;
    public int myInfo;
    public int opponentInfo;
    public int roomNum;
    public UserInfo userInfo;
    private TransportTCP socket;
    public bool userInfoFlag;

    // 매치메이킹 State 열거형.
    public enum MatchingState { Nothing, WaitMatchingResult, SelectMatchingResult, AcceptMatchingResult, RefuseMatchingResult}

    // 매치메이킹 진행 State
    [SerializeField]
    private MatchingState matchingState;
    public MatchingState MatchState { get { return matchingState; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        userInfoFlag = false;

        // 현재 매치메이킹 진행 state 초기화
        matchingState = MatchingState.Nothing;

        networkManager = GetComponent<MatchingNetworkManager>();
        networkManager.RegisterReceiveNotification(PacketId.MatchingResponse,
            OnReceiveMatchingResponsePacket);
        networkManager.RegisterReceiveNotification(PacketId.MatchingRetry,
            OnReceiveMatchingRetryPacket);
        networkManager.RegisterReceiveNotification(PacketId.MatchingReject,
            OnReceiveMatchingRejectPacket);
        networkManager.RegisterReceiveNotification(PacketId.MatchingComplete,
            OnReceiveMatchingCompletePacket);

    }

    // 업데이트 처리 후 late update 실행.
    void LateUpdate()
    {
        if (userInfoFlag == false)
        {
            SendData(userInfo.userData.id);
            userInfoFlag = true;
        }
    }

    private void SendData(string msg)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
        networkManager.SendData(buffer);
        Debug.Log("매칭 유저 정보 전송");
    }

    private void ChangeMatchingState(MatchingState st)
    {
        matchingState = st;
    }

    // 매칭 시작 버튼 클릭 함수
    public void MatchingRequest()
    {
        SendLocalMatchingRequest();
        Debug.Log("매칭 해주세요 버튼 클릭");
    }
    
    // 매칭중이고 상대의 응답을 기다리는 중에 나타나는 '매칭취소' 버튼 클릭 함수
    public void MatchingCancel()
    {
        Debug.Log("매칭 취소 하겠습니다 버튼 클릭 ");
        SendLocalMatchingCancel();

        ChangeMatchingState(MatchingState.Nothing);
    }

    // 매치메이킹 수락
    public void AccpetMatchMakingResult()
    {
        Debug.Log("매칭 결과 수락 버튼 클릭");
        SendLocalMatchingAccept();


        ChangeMatchingState(MatchingState.AcceptMatchingResult);
        // 상대방 매칭 대기중 메시지 띄우기
        // 게임 씬으로 이동
    }

    // 매치메이킹 거절
    public void RefuseMatchMakingResult()
    {
        Debug.Log("매칭 결과 거절 버튼 클릭");
        // 초기 로비 화면으로 돌아감
        SendLocalMatchingReject();
        ChangeMatchingState(MatchingState.RefuseMatchingResult);
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

    //매칭 취소
    public void SendLocalMatchingCancel()
    {
        MatchingCancelData matchingCancelData = new MatchingCancelData();
        matchingCancelData.myInfo = 0;
        MatchingCancelPacket packet = new MatchingCancelPacket(matchingCancelData);
        networkManager.SendReliable<MatchingCancelData>(packet);
    }
    //매칭 수락
    public void SendLocalMatchingAccept()
    {
        MatchingDecisionData matchingDecisionData = new MatchingDecisionData();
        matchingDecisionData.decision = MatchingDecision.Accept;
        matchingDecisionData.myinfo = myInfo;
        MatchingDecisionPacket packet = new MatchingDecisionPacket(matchingDecisionData);
        networkManager.SendReliable<MatchingDecisionData>(packet);
    }

    //매칭 거절
    public void SendLocalMatchingReject()
    {
        MatchingDecisionData matchingDecisionData = new MatchingDecisionData();
        matchingDecisionData.decision = MatchingDecision.Reject;
        matchingDecisionData.myinfo = myInfo;
        MatchingDecisionPacket packet = new MatchingDecisionPacket(matchingDecisionData);
        networkManager.SendReliable<MatchingDecisionData>(packet);
    }

    //매칭 응답 패킷 받기
    //매칭 중, 매칭 잡힘 처리
    public void OnReceiveMatchingResponsePacket(PacketId id, byte[] data)
    {
        //MatchingPacketId에 대한 처리해주기
        MatchingResponsePacket packet = new MatchingResponsePacket(data);
        MatchingResponseData packetData = packet.GetPacket();

        if (packetData.request == MatchingPacketId.MatchingResponse)
        {
            if (packetData.result == MatchingResult.Success)
            {
                Debug.Log("매칭중~~~~~~~");
                //매칭 중인것을 클라이언트에 표시해야 함
                ChangeMatchingState(MatchingState.WaitMatchingResult);
            }
            else
            {
                Debug.Log("매칭 실패");
                ChangeMatchingState(MatchingState.Nothing);
            }
        }
        //매칭이 잡힘 
        else if(packetData.request == MatchingPacketId.MatchingCatch)
        {
            myInfo = -1;

            if(packetData.result == MatchingResult.Success)
            {
                myInfo = packetData.myInfo;
                //내 정보 상대 정보 저장.
                //수락 여부 버튼 띄우기.

                ChangeMatchingState(MatchingState.SelectMatchingResult);
            
            }
            else
            {
                Debug.Log("매칭 잡힘 실패");
                ChangeMatchingState(MatchingState.Nothing);
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
            ChangeMatchingState(MatchingState.WaitMatchingResult);
        }
    }

    public void OnReceiveMatchingRejectPacket(PacketId id, byte[] data)
    {
        Debug.Log("rejectPacket");
        MatchingRejectPacket packet = new MatchingRejectPacket(data);
        MatchingRejectData packetData = packet.GetPacket();

        if(packetData.result == MatchingResult.Success)
        {
            Debug.Log("매칭 거절 됨");
            ChangeMatchingState(MatchingState.Nothing);
        }
    }

    //둘 다 매칭을 수락했을 때의 메세지
    public void OnReceiveMatchingCompletePacket(PacketId id, byte[] data)
    {
        userInfo.userData.roomNum = -1;

        MatchingCompletePacket packet = new MatchingCompletePacket(data);
        MatchingCompleteData packetData = packet.GetPacket();
        Debug.Log("둘 다 매칭 수락 게임을 시작합니다.");
        userInfo.userData.roomNum = (int)packetData.roomId;     //방번호 저장
        userInfo.userData.playerCamp = (int)packetData.playerCamp;
        //MatchingResponseWaitUI.transform.GetChild(1).GetComponent<Text>().text = "잠시 후 게임씬으로 넘어갑니다..";
        SceneManager.LoadScene("EQ_Test");
        //방번호와 내 정보 수신

    }
    //매칭 수락후 서버에서 게임 접속 메세지
    public void OnReceiveGameStart(PacketId id, byte[] data)
    {

    }
}
