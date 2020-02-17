using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager_Lobby : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject chattingPanel;
    [SerializeField] private GameObject profilePanel;

    [Header("Profile")]
    [SerializeField] private Text userNameText;
    [SerializeField] private Text userEmailText;
    [SerializeField] private Text winCntText;
    [SerializeField] private Text loseCntText;
    [SerializeField] private Text winRateText;
    [SerializeField] private Text killCntText;
    [SerializeField] private Text lostCntText;
    [SerializeField] private Text totalDmgText;

    [Header("Buttons")]
    [SerializeField] private GameObject matchBtn;
    [SerializeField] private GameObject cancelBtn;
    [SerializeField] private GameObject acceptBtn;
    [SerializeField] private GameObject declineBtn;

    [SerializeField] private GameObject matchTimer;
    [SerializeField] private GameObject matchCompletePanel;
    [SerializeField] private GameObject matchWaitingPanel;

    private MatchingManager matchingManager;
    private UserInfo userInfo;

    private void Start()
    {
        StartAnimation();

        matchingManager = GameObject.Find("MatchingManager").GetComponent<MatchingManager>();
        userInfo = GameObject.Find("DataObject").GetComponent<UserInfo>();

        matchBtn.GetComponent<Button>().onClick.AddListener(matchingManager.MatchingRequest);
        cancelBtn.GetComponent<Button>().onClick.AddListener(matchingManager.MatchingCancel);
        acceptBtn.GetComponent<Button>().onClick.AddListener(matchingManager.AccpetMatchMakingResult);
        declineBtn.GetComponent<Button>().onClick.AddListener(matchingManager.RefuseMatchMakingResult);

        matchBtn.SetActive(true);
        cancelBtn.SetActive(false);
        matchTimer.SetActive(false);
        matchCompletePanel.SetActive(false);
        matchWaitingPanel.SetActive(false);

        ShowProfile();
    }

    private void Update()
    {
        ShowMatchingUI();
    }

    // 매치메이킹 요청 등의 결과와 현재 State에 따라 화면에 출력되는 UI를 결정하는 함수.
    private void ShowMatchingUI()
    {
        MatchingManager.MatchingState state = matchingManager.MatchState;

        switch (state)
        {
            case MatchingManager.MatchingState.Nothing:
                matchBtn.SetActive(true);
                cancelBtn.SetActive(false);
                matchTimer.SetActive(false);
                matchCompletePanel.SetActive(false);
                matchWaitingPanel.SetActive(false);
                break;

            case MatchingManager.MatchingState.WaitMatchingResult:
                matchBtn.SetActive(false);
                cancelBtn.SetActive(true);
                matchTimer.SetActive(true);
                matchCompletePanel.SetActive(false);
                matchWaitingPanel.SetActive(false);
                break;

            case MatchingManager.MatchingState.SelectMatchingResult:
                matchBtn.SetActive(false);
                cancelBtn.SetActive(false);
                matchTimer.SetActive(false);
                matchCompletePanel.SetActive(true);
                matchWaitingPanel.SetActive(false);
                break;

            case MatchingManager.MatchingState.AcceptMatchingResult:
                matchBtn.SetActive(false);
                cancelBtn.SetActive(false);
                matchTimer.SetActive(false);
                matchCompletePanel.SetActive(false);
                matchWaitingPanel.SetActive(true);
                matchWaitingPanel.GetComponentInChildren<Text>().text = "Waiting For Opponent...";
                break;

            case MatchingManager.MatchingState.RefuseMatchingResult:
                matchBtn.SetActive(false);
                cancelBtn.SetActive(false);
                matchTimer.SetActive(false);
                matchCompletePanel.SetActive(false);
                matchWaitingPanel.SetActive(true);
                matchWaitingPanel.GetComponentInChildren<Text>().text = "Cancel...";
                break;
        }
    }

    private void ShowProfile()
    {
        UserData userData = userInfo.userData;
        UserPlayData userPlayData = userInfo.userPlayData;

        userNameText.text = userData.username;
        userEmailText.text = userData.email;
        winCntText.text = userPlayData.victory.ToString();
        loseCntText.text = userPlayData.lose.ToString();
        winRateText.text = userPlayData.victory + userPlayData.lose == 0 ?
            "- %" : ((float)userPlayData.victory / (userPlayData.victory + userPlayData.lose)).ToString("0.00%");
        killCntText.text = userPlayData.kill.ToString();
        lostCntText.text = userPlayData.death.ToString();
        totalDmgText.text = userPlayData.damage.ToString();
    }

    private void StartAnimation()
    {
        float duration = 1f;
        float distance = 2000f;

        Vector2 endPoint_ChattingPanel = chattingPanel.transform.position;
        Vector2 endPoint_MatchBtn = matchBtn.transform.position;
        Vector2 endPoint_ProfilePanel = profilePanel.transform.position;

        chattingPanel.transform.position += new Vector3(distance, 0, 0);
        matchBtn.transform.position += new Vector3(distance, 0, 0);
        profilePanel.transform.position -= new Vector3(distance, 0, 0);

        chattingPanel.transform.DOMove(endPoint_ChattingPanel, duration);
        matchBtn.transform.DOMove(endPoint_MatchBtn, duration);
        profilePanel.transform.DOMove(endPoint_ProfilePanel, duration);
    }
}
