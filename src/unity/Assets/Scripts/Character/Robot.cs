using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    public enum State { Idle, Move, Attack, CC, Die }

    public int index;
    public Status status;
    public bool is1P;

    [SerializeField] private State state;
    [SerializeField] private Character target = null;
    [SerializeField] private LayerMask contactLayer;

    public RobotAttack attack;

    private NavMeshAgent agent;
    private List<Vector3> destinations;
    private int destFlag;
    private bool isAttacking = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        FindNearestTarget();
        StateMachine();
    }

    public void InitialSetting(int index, bool is1P, List<Vector3> destinations)
    {
        this.index = index;
        this.is1P = is1P;
        this.destinations = destinations;
        status.ChangeStat(StatusType.CHP, status.MHP);

        if (is1P)
        {
            transform.position = destinations[0];
            destFlag = 1;
            agent.destination = destinations[destFlag];
        }
        else
        {
            transform.position = destinations[destinations.Count - 1];
            destFlag = destinations.Count - 2;
            agent.destination = destinations[destFlag];
        }

        state = State.Idle;
        attack.Initialize();
    }

    private void FindNearestTarget()
    {
        target = null;

        Collider[] colls = Physics.OverlapSphere(transform.position, attack.range, contactLayer);

        float nearestDis = 9999999;

        foreach (Collider coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            if (is1P == coll.transform.GetComponent<Character>().is1P) continue;

            if (Vector3.Distance(coll.transform.position, transform.position) < nearestDis)
            {
                nearestDis = Vector3.Distance(coll.transform.position, transform.position);
                target = coll.GetComponent<Character>();
            }
        }
    }

    private void Attack()
    {
        Vector3 dir = target.transform.position - transform.position;
        // 발사체 오브젝트 교체
        // ChangeProjectile();
        StartCoroutine(attack.Use(this, dir.normalized));
    }

    private void StateMachine()
    {
        if (status.CHP <= 0)
        {
            state = State.Die;
        }
        else if (status.HardCC != HardCCType.None)
        {
            // CC기에 따른 행동
            // 현재는 Stun 밖에 없음
            agent.destination = transform.position;

            // 20 02 09 CharacterState에 CC 추가.
            state = State.CC;
        }
        else if (target)
        {
            state = State.Attack;
        }
        else
        {
            state = State.Move;
        }

        StateAction();
    }

    private void StateAction()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Move:
                // 20 02 07 상대 캐릭터의 속도를 보정된 값으로 지정.
                // agent.speed = is1P ? status.SPD : MovingManager.instance.GetInterpolatedSpeed(index);
                agent.speed = status.SPD;
                if (agent.remainingDistance < 3f)
                {
                    destFlag += is1P ? 1 : -1;
                    agent.destination = destinations[destFlag];
                }
                break;
            case State.Attack:
                agent.speed = 0;
                Attack();
                break;
            case State.Die:
                OnDeadStateActivate();
                break;
            // 20 02 09 CC switch-case 추가.
            case State.CC:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 파라미터로 들어온 효과들을 자신에게 적용하는 함수.
    /// duration이 0이면 영구적, 아니라면 일시적으로 적용한다.
    /// </summary>
    /// <param name="effects">적용할 효과들</param>
    public void Apply(List<EffectResult> effects)
    {
        foreach (EffectResult effect in effects)
        {
            //Debug.Log(name + "에게 " + effect.statusType.ToString() + "을 " + effect.amount + "만큼 변화시키는 효과가 " + effect.duration + "동안 적용!");
            if (effect.hardCCType == HardCCType.None)
            {
                if (effect.duration == 0)
                {
                    status.ChangeStat(effect.statusType, effect.amount);
                }
                else
                {
                    StartCoroutine(TempEffect(effect));
                }
            }
            else
            {
                StartCoroutine(CCEffect(effect));
            }
        }
    }

    /// <summary>
    /// 일시적인 효과(버프/디버프)를 적용하는 함수.
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    private IEnumerator TempEffect(EffectResult effect)
    {
        status.ChangeStat(effect.statusType, effect.amount);

        yield return new WaitForSeconds(effect.duration);

        status.ChangeStat(effect.statusType, -effect.amount);
    }

    private IEnumerator CCEffect(EffectResult effect)
    {
        status.ApplyCC(effect.hardCCType);

        yield return new WaitForSeconds(effect.duration);

        status.ApplyCC(HardCCType.None);
    }

    // EffectController에서 Robot.State 받아오게끔 하는 함수. 200130 작성
    public State GetCharacterState()
    {
        return state;
    }

    // 20 02 10 Die 상황시 컴포넌트 비활성화
    private void OnDeadStateActivate()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        List<Canvas> canvasList = new List<Canvas>(transform.GetComponentsInChildren<Canvas>());
        for (int i = 0; i < canvasList.Count; i++)
        {
            canvasList[i].enabled = false;
        }
        enabled = false;
    }
}
