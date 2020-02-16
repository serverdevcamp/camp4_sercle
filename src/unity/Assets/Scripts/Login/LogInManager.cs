using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class LogInManager : MonoBehaviour
{
    [SerializeField] private InputField idInput;
    [SerializeField] private InputField pwInput;
    [SerializeField] private GameObject infoText;

    private UserInfo userInfo;
    private HTTPManager httpManager;
    private APIManager apiManager;
    private void Start()
    {
        userInfo = GameObject.Find("DataObject").GetComponent<UserInfo>();
        httpManager = new HTTPManager();
        apiManager = new APIManager();
        infoText.SetActive(false);
    }

    // 로그인 버튼 클릭시 이벤트 발생 함수
    public void SignInBtn()
    {
        //userInfo.userData = JsonUtility.FromJson<UserData>(httpManager.LoginReq("bluetwintail@naver.com", "1q2w3e4r!"));
        userInfo.userData = JsonUtility.FromJson<UserData>(httpManager.LoginReq(idInput.text, pwInput.text));

        //로그인 성공시
        if (userInfo.userData.login == "true")
        {
            //유저의 게임 플레이 데이터(킬, 데스, 승, 패,  데미지...)
            userInfo.userPlayData = JsonUtility.FromJson<UserPlayData>(httpManager.UserDataReq(userInfo.userData.id));
            //업적 리스트 가져옴
            userInfo.AchieveData = apiManager.GetAchieveData(httpManager.AchieveListDataReq());
            //유저가 달성한 업적 리스트 가져옴
            userInfo.userAchieveData = apiManager.GetAchieveData(httpManager.UserAchieveListDataReq(userInfo.userData.id));
            SceneManager.LoadScene("Lobby");
        }
        else if (userInfo.userData.login == "overlap")
        {
            Debug.Log("이미 접속해있습니다.");
            infoText.SetActive(true);
        }
        else
        {
            Debug.Log("아이디 또는 비밀번호가 틀렸습니다.");
            infoText.SetActive(true);
        }
    }

    // 회원가입 버튼 클릭시 이벤트 발생 함수
    public void SignUpBtn()
    {
        // 회원가입 페이지 or 씬으로 이동.
    }
}
