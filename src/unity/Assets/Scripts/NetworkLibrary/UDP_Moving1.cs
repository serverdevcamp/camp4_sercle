/*
 * 키 동기화 안하고 그저 마우스 입력값으로 상대와 자신을 옮기는 코드
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDP_Moving1 : MonoBehaviour
{
    // UDP
    private TransportUDP socket = new TransportUDP();
   
    // 내 캐릭터
    public Transform player;

    // 상대 캐릭터
    public Transform opponent;

    // server ip, port
    //private string address = "127.0.0.1";
    //private int port = 3098;

    // set coroutine
    private Coroutine myMove;

    // opponent coroutine
    private Coroutine oppMove;

    public InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
         myMove = null;
        oppMove = null;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();


        // 서버에 접속
        //socket.Connect(address, port);
        // 통신 스레드 시작. Thread를 쓸 것이냐, Coroutine을 쓸 것이냐 선택해야함. 스레드는 일시정지 불가
        // socket.LaunchThread();
    }

    // Update is called once per frame
    public void GameUpdate()
    {

        if (inputManager.GetMouseInputData(0).mouseButtonLeft)
        {
            Vector3 targetPos = Vector3.zero;
        
            
            targetPos.x = inputManager.GetMouseInputData(0).mousePositionX;
            targetPos.y = inputManager.GetMouseInputData(0).mousePositionY;
            targetPos.z = inputManager.GetMouseInputData(0).mousePositionZ;
            if (myMove != null)
                StopCoroutine(myMove);
            // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
            myMove = StartCoroutine(Move(targetPos, player));
        }
        if (inputManager.GetMouseInputData(1).mouseButtonLeft)
        {
            Vector3 targetPos = Vector3.zero;


            targetPos.x = inputManager.GetMouseInputData(1).mousePositionX;
            targetPos.y = inputManager.GetMouseInputData(1).mousePositionY;
            targetPos.z = inputManager.GetMouseInputData(1).mousePositionZ;

            if (oppMove != null)
                StopCoroutine(oppMove);
            // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
            oppMove = StartCoroutine(Move(targetPos, opponent));
        }
    }

    IEnumerator Move(Vector3 target, Transform player)
    {
        while (target.x != (int)player.position.x || target.z != (int)player.position.z)
        {
            player.position = Vector3.MoveTowards(player.position, target, Time.deltaTime * 5);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
