/*
 * 키 동기화 안하고 그저 마우스 입력값으로 상대와 자신을 옮기는 코드
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDP_Moving2 : MonoBehaviour
{
   
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


   
    }

    // Update is called once per frame
    public void GameUpdate()
    {

        if (inputManager.GetMouseInputData(0).mouseButtonLeft)
        {
            // 애니메이션
            player.GetComponent<Animator>().SetBool("Running", true);


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
            // 애니메이션
            opponent.GetComponent<Animator>().SetBool("Running", true);
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
        while ((int)target.x != (int)player.position.x || (int)target.z != (int)player.position.z)
        {
            // 회전
            var rotate = Quaternion.LookRotation(target);
            player.rotation = Quaternion.Slerp(player.rotation, rotate, Time.deltaTime * 4);
            // 이동
            player.position = Vector3.MoveTowards(player.position, target, Time.deltaTime * 5);
            yield return new WaitForSeconds(0.02f);
        }
        // 목적지에 도달했다면 애니메이션 종료
        player.GetComponent<Animator>().SetBool("Running", false);
    }
}
