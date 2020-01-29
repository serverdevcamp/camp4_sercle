/*
 * 상대가 보낸 패킷을 분석하는 작업 후
 * 어떤 Update함수에 그 작업을 뿌려줄지 결정해주는 코드 작성해야 한다.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PT4_GameManager : MonoBehaviour
{
    public Transform player;
    public Transform opponent;


    // set coroutine
    private Coroutine myMove;

    // opponent coroutine
    private Coroutine oppMove;

    // attack coroutine
    private Coroutine attackCor;

    // hit coroutine
    private Coroutine hitCor;

    private NetworkManager transport;

    private InputManager inputManager;

    private MovingManager movingManager;
    NavMeshAgent agent1;
    NavMeshAgent agent2;
    

    // Start is called before the first frame update
    void Start()
    {
        transport = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        movingManager = GameObject.Find("MovingManager").GetComponent<MovingManager>();
        agent1 = GameObject.Find("player1").GetComponent<NavMeshAgent>();
        agent1.autoBraking = false;
        agent2 = GameObject.Find("player2(opp)").GetComponent<NavMeshAgent>();
        agent2.autoBraking = false;
    }


    private void Update()
    {
        //GameMovingUpdate();
        //GameAttackUpdate();
        //ParseOpponentMsg();

        if (agent1.remainingDistance <= 0.1f)
        {
            agent1.isStopped = true;
            player.GetComponent<Animator>().SetBool("Running", false);
        }
        if(agent2.remainingDistance <= 0.1f)
        {
            agent2.isStopped = true;
            opponent.GetComponent<Animator>().SetBool("Running", false);
            agent2.speed =3.5f;
        }
    }


    IEnumerator AttackCor(Transform player)
    {
        if (!player.GetComponent<Animator>().GetBool("Attack"))
        {
            player.GetComponent<Animator>().SetBool("Attack", true);
            // 회전
            var rotate = Quaternion.LookRotation(opponent.transform.position);
            player.rotation = Quaternion.Slerp(player.rotation, rotate, Time.deltaTime * 15);
            yield return new WaitForSeconds(0.02f);
            player.GetComponent<Animator>().SetBool("Attack", false);
            yield return new WaitForSeconds(2.5f);
            attackCor = null;
        }
        
    }

    private void ParseOpponentMsg()
    {
        // 명시적 버퍼 사이즈
        byte[] buffer = new byte[1024];
        //int recvSize = transport.Receive(ref buffer, buffer.Length);
        int recvSize = 3;
        // 아직 '전부' 수신되지 않았음.
        if (recvSize < 0)
        {
            return;
        }

        string msg = System.Text.Encoding.UTF8.GetString(buffer);
        string[] pos = msg.Split(';');
        Vector3 targetPos = new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));


        if (pos[0] == "Attack")
        {
            RecvAttackMsg(targetPos);
        }
        else if(pos[0] == "Moving")
        {
            RecvMovingMsg(targetPos);
            
        }
       
    }

    private void GameAttackUpdate()
    {
        if (Vector3.Distance(player.position, opponent.position) <= 3f )
        {
            if (attackCor == null && !player.GetComponent<Animator>().GetBool("Attack"))
            {
                // 내 캐릭터가 공격모션취함
                attackCor = StartCoroutine(AttackCor(player));
                // 이제 상대 단말에서도 내 캐릭터가 공격모션 취함
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Attack;" + 0.ToString() + ';' + 0.ToString() + ';' + 0.ToString());
               // transport.Send(buffer, buffer.Length);

                PlayHitAnim(opponent);
            }
        }
        else
        {
            attackCor = null;
        }
    }

    IEnumerator HitCor(Transform player)
    {
        player.GetComponent<Animator>().SetTrigger("Hit");
        yield return new WaitForSeconds(0.02f); //그냥넣어봄
        hitCor = null;
    }
    private void PlayHitAnim(Transform player)
    {
        if (hitCor != null)
        {
            StopCoroutine(hitCor);
        }
        hitCor = StartCoroutine(HitCor(player));


    }

    public void RecvMovingMsg(Vector3 targetPos)
    {
        
        print("Recv Moving " + targetPos);

        // 애니메이션
        opponent.GetComponent<Animator>().SetBool("Running", true);

        //if (oppMove != null)
        //    StopCoroutine(oppMove);
        //// 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
        //oppMove = StartCoroutine(Move(targetPos, opponent));

        

        agent2.destination = targetPos;
        agent2.speed = GetSyncedMovingSpeed(targetPos);
        opponent.GetComponent<Animator>().SetFloat("RunningSpeed", 1f + GameObject.Find("SyncManager").GetComponent<SyncManager>().GetAvgRemoteRtt());
        agent2.isStopped = false;
    }

    // 지연시간이 고려된 속도 반환
    private float GetSyncedMovingSpeed(Vector3 targetPos)
    {
        // 거리 계산
        float d = Vector3.Distance(opponent.position, targetPos);
        // 시간 계산
        float t = d / agent2.speed;
        // 계산용 시간 계산
        float ct = t - GameObject.Find("SyncManager").GetComponent<SyncManager>().GetAvgRemoteRtt();
        Debug.Log(d / ct);
        // 계산된 속력 반환
        return d / ct;
    }

    private void RecvAttackMsg(Vector3 targetPos)
    {
        print("Recv Attacking " + targetPos);

        if (attackCor != null)
            StopCoroutine(attackCor);
        // 상대 캐릭터의 공격 애니메이션
        attackCor = StartCoroutine(AttackCor(opponent));

        PlayHitAnim(player);
    }
    public void GameMovingUpdate(Vector3 targetPos)
    {
        Debug.Log(targetPos);

        //if (myMove != null)
        //    StopCoroutine(myMove);
        //// 위치를 얻었으므로 행동 시작. 프로토타입에서는 '이동' 행동
        //myMove = StartCoroutine(Move(targetPos, player));

       
        
        agent1.destination = targetPos;
        agent1.isStopped = false;
        player.GetComponent<Animator>().SetBool("Running",true);

    }

    IEnumerator Move(Vector3 target, Transform player)
    {
        while (target.x - player.position.x >= Mathf.Epsilon || target.z - player.position.z >= Mathf.Epsilon)
        {
            // 회전
            var rotate = Quaternion.LookRotation(target);
            player.rotation = Quaternion.Slerp(player.rotation, rotate, Time.deltaTime * 15);
            // 이동
            player.position = Vector3.MoveTowards(player.position, target, Time.deltaTime * 5);
            yield return new WaitForSeconds(0.02f);
        }
        // 목적지에 도달했다면 애니메이션 종료
        player.GetComponent<Animator>().SetBool("Running", false);
    }
}
