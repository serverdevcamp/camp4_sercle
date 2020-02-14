using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    public enum State { Idle, Move, Attack, Skill, CC, Die }

    [Header("Robot Info")]
    [SerializeField] private int index;
    [SerializeField] private Status status;
    [SerializeField] private bool is1P;
    [SerializeField] private List<Vector3> destinations;
    [SerializeField] private int destFlag;

    [Header("Attack Info")]
    [SerializeField] private State state;
    [SerializeField] private Robot target = null;
    [SerializeField] private LayerMask contactLayer;
    [SerializeField] private Attack attack;

    private NavMeshAgent agent;

    public int Index { get { return index; } }
    public Status GetStatus { get { return status; } }
    public bool Is1P { get { return is1P; } }
    public State GetState { get { return state; } }
    public Attack MyAttack { get { return attack; } }

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
        state = State.Idle;
        status.ChangeStatTo(StatusType.CHP, status.MHP);

        if (is1P) destFlag = 1;
        else destFlag = destinations.Count - 2;

        agent.destination = destinations[destFlag];
    }

    private void FindNearestTarget()
    {
        target = null;

        Collider[] colls = Physics.OverlapSphere(transform.position, attack.range, contactLayer);

        float nearestDis = 9999999;

        foreach (Collider coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            if (is1P == coll.transform.GetComponent<Robot>().is1P) continue;

            if (target == null || Vector3.Distance(coll.transform.position, transform.position) < nearestDis)
            {
                nearestDis = Vector3.Distance(coll.transform.position, transform.position);
                target = coll.GetComponent<Robot>();
            }
        }
    }

    private void StateMachine()
    {
        if (status.CHP <= 0) state = State.Die;
        else if (status.CC != CCType.None) state = State.CC;
        else if (target) state = State.Attack;
        else if (agent.remainingDistance > agent.stoppingDistance) state = State.Move;
        else state = State.Idle;

        StateAction();
    }

    private void StateAction()
    {
        switch (state)
        {
            case State.Idle:
                agent.speed = 0;
                break;
            case State.Move:
                // 20 02 07 상대 캐릭터의 속도를 보정된 값으로 지정.
                // isFriend ? status.SPD : MovingManager.instance.GetInterpolatedSpeed(index);
                agent.speed = status.SPD;
                break;
            case State.Attack:
                agent.speed = 0;
                AttackActivate();
                break;
            case State.Die:
                OnDeadStateActivate();
                break;
            case State.CC:
                break;
            default:
                break;
        }
    }

    private void AttackActivate()
    {
        Vector3 dir = target.transform.position - transform.position;
        // 발사체 오브젝트 교체
        // ChangeProjectile();
        StartCoroutine(attack.Use(this, dir.normalized));
    }

    /// <summary>
    /// 파라미터로 들어온 효과들을 자신에게 적용하는 함수.
    /// duration이 0이면 영구적, 아니라면 일시적으로 적용한다.
    /// </summary>
    /// <param name="effects">적용할 효과들</param>
    public void Apply(SkillEffect effect)
    {
        if (effect.ccType != CCType.None) StartCoroutine(CCEffect(effect));
        else if (effect.duration != 0) StartCoroutine(TempEffect(effect));
        else status.ChangeStat(effect.statusType, effect.amount);
    }

    /// <summary>
    /// 일시적인 효과(버프/디버프)를 적용하는 함수.
    /// </summary>
    /// <param name="effect">적용할 효과</param>
    /// <returns></returns>
    private IEnumerator TempEffect(SkillEffect effect)
    {
        status.ChangeStat(effect.statusType, effect.amount);

        yield return new WaitForSeconds(effect.duration);

        status.ChangeStat(effect.statusType, -effect.amount);
    }

    /// <summary>
    /// CC기를 적용하는 함수
    /// </summary>
    /// <param name="effect">적용할 효과</param>
    /// <returns></returns>
    private IEnumerator CCEffect(SkillEffect effect)
    {
        status.ApplyCC(effect.ccType);

        yield return new WaitForSeconds(effect.duration);

        status.ApplyCC(CCType.None);
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
        this.enabled = false;
    }
}
