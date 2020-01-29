using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class GameManager : MonoBehaviour
{
    // TCP
    private TransportTCP socket = new TransportTCP();

    // 현재 누구의 턴인가
    public bool turn;

    // 서버가 정해준 먼저둘 플레이어
    public bool you;

    // local turn
    public bool localTurn;
    // remote turn
    public bool remoteTurn;


    // 내 캐릭터
    public Transform player;

    // 상대 캐릭터
    public Transform opponent;

    // server ip, port
    private string address = "127.0.0.1";
    private int port = 3098;

    // set coroutine
    private Coroutine myMove;

    // opponent coroutine
    private Coroutine oppMove;

    // Start is called before the first frame update
    void Start()
    {
        if (you)
        {
            localTurn = true;
            remoteTurn = false;
        }
        else
        {
            localTurn = false;
            remoteTurn = true;
        }

        // 서버에서 정해줘야 함 순서는
        //turn = false;
        myMove = null;
        oppMove = null;
        
            
        // 서버에 접속
        socket.Connect(address, port);
        // 통신 스레드 시작. Thread를 쓸 것이냐, Coroutine을 쓸 것이냐 선택해야함. 스레드는 일시정지 불가
        //socket.LaunceThread();
    }

    // Update is called once per frame
    void Update()
    {
        //MainLogic();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f))
            {
                targetPos = hit.point;
            }
            // 좌표를 상대에게 전달.
            //byte[] buffer = V3ToStr(targetPos);
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
            // 송신 엔드포인트
            //Debug.Log(targetPos + " " + buffer.Length);
            /*
            byte[] buffer = new byte[3];
            buffer[0] = (byte)((int)targetPos.x);
            buffer[1] = (byte)((int)targetPos.y);
            buffer[2] = (byte)((int)targetPos.z);
            */
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(targetPos.x.ToString() + ';' + targetPos.y.ToString() + ';' + targetPos.z.ToString());
            socket.Send(buffer, buffer.Length);
            if (myMove != null)
                StopCoroutine(myMove);
            // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
            myMove = StartCoroutine(Move(targetPos, player));
        }
        if (true)
        {
            // 명시적 버퍼 사이즈
            byte[] buffer = new byte[1024];
            int recvSize = socket.Receive(ref buffer, buffer.Length);

            // 아직 '전부' 수신되지 않았음.
            if (recvSize <= 0)
            {
                return;
            }

            string msg = System.Text.Encoding.UTF8.GetString(buffer);
            string[] pos = msg.Split(';');

            // 수신 정보를 위치로 변환
            //Vector3 targetPos = StrToV3(buffer);
            Vector3 targetPos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            print(targetPos);
            if (oppMove != null)
                StopCoroutine(oppMove);
            // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
            oppMove = StartCoroutine(Move(targetPos, opponent));
           
        }

        

    }

    // 자신의 턴일때 할 일(마우스 클릭하고, 그 위치를 상대에게 전송)
    bool DoOwnTurn()
    {
        bool isClicked = Input.GetMouseButtonDown(0);
        // 아무것도 눌리지 않은 경우 아무행동 안함
        if(isClicked == false)
        {
            return false;
        }


        Vector3 targetPos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f))
        {
            targetPos = hit.point;
        }
        // 좌표를 상대에게 전달.
        byte[] buffer = V3ToStr(targetPos);
        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
        // 송신 엔드포인트
        Debug.Log(targetPos + " " + buffer.Length);
        socket.Send(buffer, buffer.Length);

        if (myMove != null)
            StopCoroutine(myMove);
        // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
        myMove = StartCoroutine(Move(targetPos, player));

        return true;
    }

    // 상대의 턴일때 할 일
    bool DoOpponentTurn()
    {
        // 명시적 버퍼 사이즈
        byte[] buffer = new byte[3];
        int recvSize = socket.Receive(ref buffer, buffer.Length);

        // 아직 '전부' 수신되지 않았음.
        if(recvSize <= 0)
        {
            return false;
        }

        // 수신 정보를 위치로 변환
        Vector3 targetPos = StrToV3(buffer);
        print(targetPos);
        if (myMove != null)
            StopCoroutine(oppMove);
        // 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
        oppMove = StartCoroutine(Move(targetPos, opponent));

        return true;
    }

    // 이동 행동
    IEnumerator Move(Vector3 target, Transform player)
    {
        while(target.x != (int)player.position.x || target.z != (int)player.position.z)
        {
            player.position = Vector3.MoveTowards(player.position, target, Time.deltaTime * 5);
            yield return new WaitForSeconds(0.02f);
        }
    }
    byte[] V3ToStr(Vector3 v3)
    {

        byte[] array = new byte[3];
        array[0] = (byte)(int)v3.x;
        array[1] = (byte)(int)v3.y;
        array[2] = (byte)(int)v3.z;
        return array;
        //return ((int)v3.x).ToString() + '.' + ((int)v3.z).ToString();
    }
    Vector3 StrToV3(byte[] bytes)
    {
        return new Vector3((int)bytes[0], (int)bytes[1], (int)bytes[2]);
    }

    private void MainLogic()
    {
        bool setMark = false;

        // 내 턴
        if (turn == localTurn)
        {
            setMark = DoOwnTurn();
        }
        // 상대의 턴
        else
        {
            setMark = DoOpponentTurn();
        }

        // 나 or 상대가 행동을 결정 or 수행하는 중이다.
        if(setMark == false)
        {
            return;
        }

        turn = (turn == true) ? false : true;
    }
}
