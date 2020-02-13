using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_Lobby : MonoBehaviour
{
    [SerializeField] private GameObject matchBtn;
    [SerializeField] private GameObject cancelBtn;
    [SerializeField] private GameObject matchTimer;
    [SerializeField] private GameObject matchCompletePanel;
    [SerializeField] private GameObject matchWaitingPanel;

    private MatchingManager matchingManager;

    private void Start()
    {
        matchingManager = GameObject.Find("MatchingManager").GetComponent<MatchingManager>();
        matchBtn.GetComponent<Button>().onClick.AddListener(matchingManager.MatchingRequest);

        matchBtn.SetActive(true);
        cancelBtn.SetActive(false);
        matchTimer.SetActive(false);
        matchCompletePanel.SetActive(false);
        matchWaitingPanel.SetActive(false);
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
}
