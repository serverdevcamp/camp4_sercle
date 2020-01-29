using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
public class Test02_Manager : MonoBehaviour
{

    // net ip addr
    public string network = "127.0.0.1";
    // port
    public int port = 3098;

    // 네트워크
    NetworkController networkController = null;
    public int[] inputbuff = new int[2];
    // 게임모드
    public GameMode gameMode;
    public enum GameMode
    {
        Ready = 0, // 접속대기
        Game, // 게임중
        Result // 결과표시
    }


    private void Awake()
    {
        // 초기 시작시 일단 멈춤
        Time.timeScale = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMode = GameMode.Ready;
    }

    private void FixedUpdate()
    {

        switch (gameMode)
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

        // 프레임 동기화를 진행해도 되는지 확인한다.
        // 네트워크컨트롤러가 있는데 종료 시그널이 왔다면 싱크 멈춤
        if (networkController != null &&  networkController.IsSync())
        {
            Debug.Log("게이멈춤");
            Debug.Log(networkController != null);
            Debug.Log(networkController.IsSync());
            networkController.ClearSync();
            // 게임 멈춤
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

    } // Fixed Update end

    private void LateUpdate()
    {


        
        if (networkController != null)
        {
            // 입력 정보를 수신한다면 true 리턴
            if (networkController.UpdateSync())
            //if (true) // 테스트. true면 정상움직임
            {
                Debug.Log("Resume");
                // 정지상태를 해제한다.
               
                Resume();
            }
            else
            {
                //Debug.Log("Suspend");
                // 입력 정보를 수신하지 않았으므로 다음 프레임을 처리할 수 없음.
                Suspend();
            }
        }
    }

    private void UpdateReady()
    {
        Debug.Log("업데이트 레디");
        // 통신 접속을 기다려 게임을 시작한다.
        if (networkController != null)
        {
            Debug.Log("컨트롤러있ㅅ삼");
            // 연결 되었다면
            if (networkController.IsConnected() == true)
            {
                Debug.Log("모드 채인지");
                gameMode = GameMode.Game;
            }
        }
    }

    private void UpdateGame()
    {
        // GameController.cs의 실 게임 로직 실시
        GetComponent<UDP_Moving1>().GameUpdate();
    }

    private void UpdateResult()
    {
        // GameController.cs의 실 게임 로직 실시
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void Suspend()
    {
        Time.timeScale = 0f;
    }

    public void IPConnect()
    {
        if (networkController == null)
        {
            //networkController = new NetworkController(network);
        }
    }
}
