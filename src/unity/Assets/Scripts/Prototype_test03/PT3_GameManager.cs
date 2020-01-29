using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PT3_GameManager : MonoBehaviour
{

    // 네트워크
    string hostAddress;
    NetworkController m_networkController = null;
    public bool isNet = false;
    public GameMode m_gameMode;     //게임 모드.
    public float m_timeScale;    //기본 타임 스케일을 기억해 둡니다.
    public int a;
    public int b;
    public enum GameMode
    {
        Ready = 0,  //접속 대기.        
        Game,       //게임 중.
        Result,     //결과 표시.
    };

    void Awake()
    {
        m_timeScale = 1;
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameMode = GameMode.Ready;

        hostAddress = "127.0.0.1";
    }

    private void FixedUpdate()
    {

        if (m_networkController != null)
        {
            a = m_networkController.mouseInputBuffer[0].Count;
            b = m_networkController.mouseInputBuffer[1].Count;
        }
        else
        {
            Debug.Log
                ("no network");
        }

        if(m_networkController == null)
        {
            isNet = false;
        }
        else
        {
            isNet = true;
        }
        switch (m_gameMode)
        {
            case GameMode.Ready:
                UpdateReady();
                break;

            case GameMode.Game:
                UpdateGame();
                break;

            case GameMode.Result:
                UpdateResult();
                break;
        }

        // 프레임 동기화를 진행해도 되는지 확인합니다.
        if (m_networkController != null && m_networkController.IsSync())
        {
            // 프레임 동기화를 진했으므로 플래그를 클리어합니다.
            m_networkController.ClearSync();

            Time.timeScale = 0; // FixedUpdate 관련 갱신은 fixedDeltaTimｅ으로 갱신되는데 2번째 이후의 호출을 막고자 =0으로 합니다.
        }
    }

    void LateUpdate()
    {
        if (m_networkController != null)
        {
            if (m_networkController.UpdateSync())
            {
                Resume();   // 정지 상태를 해제합니다.
            }
            else
            {
                // 입력 정보를 수신하지 않았으므로 다음 프레임을 처리할 수 없습니다.
                Suspend();
            }
        }
    }

    //접속 대기.
    void UpdateReady()
    {
        // 통신 접속을 기다려 게임을 시작합니다.
        if (m_networkController != null)
        {
            
            if (m_networkController.IsConnected() == true)
            {
                //NetworkController.HostType hostType = m_networkController.GetHostType();
                //GameStart(hostType == NetworkController.HostType.Server);
                Debug.Log("네트워크 접속 완료");
                m_gameMode = GameMode.Game;
            }
        }
    }


    //게임 중
    void UpdateGame()
    {

        GetComponent<UDP_Moving2>().GameUpdate();

        //GameObject gameController = GameObject.Find(m_gameControllerPrefab.name);
        //if (gameController == null)
        //{
        //    gameController = Instantiate(m_gameControllerPrefab) as GameObject;
        //    gameController.name = m_gameControllerPrefab.name;
        //    GameObject.Find("BGM").GetComponent<AudioSource>().Play();    //BGM재생.
        //    return;
        //}

        //if (gameController.GetComponent<GameController>().IsEnd())
        //{
        //    m_networkController.SuspendSync();
        //    if (m_networkController.IsSuspned() == true)
        //    {
        //        m_gameMode = GameMode.Result;
        //    }
        //}
    }


    //
    void UpdateResult()
    {
        ////결과를 표시하고 승부를 낸다한다.
        //gameobject resultcontroller = gameobject.find(m_resultcontrollerprefab.name);
        //if (resultcontroller == null)
        //{
        //    resultcontroller = instantiate(m_resultcontrollerprefab) as gameobject;
        //    resultcontroller.name = m_resultcontrollerprefab.name;
        //    gameobject.find("bgm").sendmessage("fadeout");    //bgm 페이드 아웃.
        //    return;
        //}
        Debug.Log("Update Result");
    }



    public void Resume()
    {
        Time.timeScale = m_timeScale;
    }

    public void Suspend()
    {
        Time.timeScale = 0;
    }
    public void IPConnect()
    {
        if (m_networkController == null)
        {
            m_networkController = new NetworkController(hostAddress, false);
        }
    }

}
