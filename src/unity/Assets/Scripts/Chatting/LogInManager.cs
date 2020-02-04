using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogInManager : MonoBehaviour
{
    UserInfo userInfo;
    HTTPManager httpManager;
    [Header("InputFields")]
    public InputField idInput;
    public InputField pwInput;

    [Header("Buttons")]
    public Button signInButton;
    public Button signUpButton;
    
    // Start is called before the first frame update
    void Start()
    {
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        httpManager = new HTTPManager();
    }
    
    // 로그인 버튼 클릭시 이벤트 발생 함수
    public void SignInBtn()
    {
        userInfo.userData = JsonUtility.FromJson<UserData>(httpManager.LoginReq(idInput.text, pwInput.text));
        Debug.Log(userInfo.userData.login);
        //로그인 성공시
        if (userInfo.userData.login == "true")
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    // 회원가입 버튼 클릭시 이벤트 발생 함수
    public void SignUpBtn()
    {
        // 회원가입 페이지 or 씬으로 이동.
    }


}
