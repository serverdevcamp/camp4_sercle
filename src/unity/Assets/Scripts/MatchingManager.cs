using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    // 매칭이 완료되었을 때 '수락' or '거절' 버튼을 눌렀을 때 화면에 등장할 UI. 
    public GameObject MatchingResponseWaitUI;
    // 매치메이킹이 완료되었을 때 화면에 등장할, 거절/수락 버튼을 가진 UI.
    public GameObject completeMatchMakingUI;
    // 매치메이킹이 완료되지 않았을 때 화면에 등장할 채팅창 등을 가진 UI. 
    public GameObject waitMatchMakingUI;
    // 매치메이킹 요청을 했고, 상대의 응답을 기다리는 상황에서 등장할 매칭 요청 취소 UI.
    public GameObject cancelMatchingRequestUI;
    // 매칭 시작 버튼(매칭 요청 버튼)의 게임 오브젝트
    public GameObject matchingRequestBtn;

    // 매치메이킹이 완료되었는지 판단하는 변수
    public bool isMatchMakingCompleted;
    public bool isMatchingResponseWait;
    // 매칭 시작 버튼이 눌렸는지 판단하는 변수
    public bool isMatchingRequestBtnClicked;

    private MatchingNetworkManager networkManager;
    public static MatchingManager instance;
    public int myInfo;
    public int opponentInfo;
    public int roomNum;

    // 매치메이킹 State 열거형.
    private enum MatchingState { Nothing, WaitMatchingResult, SelectMatchingResult, AcceptMatchingResult, RefuseMatchingResult}

    // 매치메이킹 진행 State
    [SerializeField]
    private MatchingState matchingState;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isMatchMakingCompleted = false;
        isMatchingResponseWait = false;
        isMatchingRequestBtnClicked = false;

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
        #region OldCodes(~20.02.06)
        //// 내가 '수락' or '거절' 버튼 클릭시
        //if (isMatchingResponseWait == true)
        //{
        //    if (!MatchingResponseWaitUI.activeSelf)
        //    {
        //        MatchingResponseWaitUI.SetActive(true);
        //        waitMatchMakingUI.SetActive(false);
        //        completeMatchMakingUI.SetActive(false);
        //    }
        //}
        //else
        //{
        //    MatchingResponseWaitUI.SetActive(false);
        //}

        //// 매칭 요청 후 상대의 응답을 기다리는 중 이라면
        //if(isMatchingRequestBtnClicked == true)
        //{
        //    if (!cancelMatchingRequestUI.activeSelf)
        //    {
        //        cancelMatchingRequestUI.SetActive(true);

        //    }
        //}


        //// 매칭이 완료 되었다면
        //if(isMatchMakingCompleted == true)
        //{
        //    // 꺼져있다면
        //    if (!completeMatchMakingUI.activeSelf)
        //    {
        //        completeMatchMakingUI.SetActive(true);
        //        waitMatchMakingUI.SetActive(false);
        //        MatchingResponseWaitUI.SetActive(false);
        //    }
        //}
        //else
        //{
        //    completeMatchMakingUI.SetActive(false);
        //}

        //if(isMatchingResponseWait == false && isMatchMakingCompleted == false)
        //{
        //    if (!waitMatchMakingUI)
        //    {
        //        waitMatchMakingUI.SetActive(true);
        //    }
        //}
        #endregion

        ShowMatchingUI();
        

    }

    // 매치메이킹 요청 등의 결과와 현재 State에 따라 화면에 출력되는 UI를 결정하는 함수.
    private void ShowMatchingUI()
    {
        switch (matchingState)
        {
            case MatchingState.Nothing:
                waitMatchMakingUI.SetActive(true);
                matchingRequestBtn.SetActive(true);
                completeMatchMakingUI.SetActive(false);
                MatchingResponseWaitUI.SetActive(false);
                cancelMatchingRequestUI.SetActive(false);
                break;

            case MatchingState.WaitMatchingResult:
                waitMatchMakingUI.SetActive(true);
                matchingRequestBtn.SetActive(false);
                completeMatchMakingUI.SetActive(false);
                MatchingResponseWaitUI.SetActive(false);
                cancelMatchingRequestUI.SetActive(true);
                break;

            case MatchingState.SelectMatchingResult:
                waitMatchMakingUI.SetActive(false);
                matchingRequestBtn.SetActive(false);
                completeMatchMakingUI.SetActive(true);
                MatchingResponseWaitUI.SetActive(false);
                cancelMatchingRequestUI.SetActive(false);
                break;

            case MatchingState.AcceptMatchingResult:
                waitMatchMakingUI.SetActive(false);
                matchingRequestBtn.SetActive(false);
                completeMatchMakingUI.SetActive(false);
                MatchingResponseWaitUI.SetActive(true);
                cancelMatchingRequestUI.SetActive(false);
                break;

            case MatchingState.RefuseMatchingResult:
                waitMatchMakingUI.SetActive(false);
                matchingRequestBtn.SetActive(false);
                completeMatchMakingUI.SetActive(false);
                MatchingResponseWaitUI.SetActive(true);
                cancelMatchingRequestUI.SetActive(false);
                break;
        }
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

        isMatchingRequestBtnClicked = true;


        

    }
    
    // 매칭중이고 상대의 응답을 기다리는 중에 나타나는 '매칭취소' 버튼 클릭 함수
    public void MatchingCancel()
    {
        Debug.Log("매칭 취소 하겠습니다 버튼 클릭 ");
        isMatchingRequestBtnClicked = false;

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

        ChangeMatchingState(MatchingState.Nothing);
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
                isMatchMakingCompleted = true;
                //내 정보 상대 정보 저장.
                //수락 여부 버튼 띄우기.

                ChangeMatchingState(MatchingState.SelectMatchingResult);
            
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
            isMatchingResponseWait = false;
            //


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
            Debug.Log("매칭 거절 됌");
            isMatchingResponseWait = false;
            isMatchMakingCompleted = false;

            ChangeMatchingState(MatchingState.Nothing);
        }
    }

    //둘 다 매칭을 수락했을 때의 메세지
    public void OnReceiveMatchingCompletePacket(PacketId id, byte[] data)
    {
        MatchingCompletePacket packet = new MatchingCompletePacket(data);
        MatchingCompleteData packetData = packet.GetPacket();
        Debug.Log("둘 다 매칭 수락 게임을 시작합니다.");
        isMatchingResponseWait = false;
        isMatchMakingCompleted = false;

        MatchingResponseWaitUI.transform.GetChild(1).GetComponent<Text>().text = "잠시 후 게임씬으로 넘어갑니다..";
    }
    //매칭 수락후 서버에서 게임 접속 메세지
    public void OnReceiveGameStart(PacketId id, byte[] data)
    {

    }
}
