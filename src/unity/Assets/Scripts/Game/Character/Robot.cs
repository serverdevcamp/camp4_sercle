using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Effects")]
    [SerializeField] private GameObject stun;
    [SerializeField] private GameObject curse;
    [SerializeField] private GameObject heal;
    [SerializeField] private GameObject frozen;
    [SerializeField] private GameObject burn;


    private NavMeshAgent agent;

    public int Index { get { return index; } }
    public Status GetStatus { get { return status; } }
    public bool Is1P { get { return is1P; } }
    public State GetState { get { return state; } }
    public Attack MyAttack { get { return attack; } }

    // 로봇 애니메이터
    private Animator robotAnimator;

    // 로봇 상태 - 부울 딕셔너리
    private Dictionary<string, bool> stateMap = new Dictionary<string, bool>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        robotAnimator = GetComponent<Animator>();
        InitStateMap();
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
                SetAnimStateMap("Idle");
                agent.speed = 0;
                break;
            case State.Move:
                SetAnimStateMap("Moving");
                agent.speed = status.SPD;
                break;
            case State.Attack:
                SetAnimStateMap("Skill_0");
                agent.speed = 0;
                AttackActivate();
                break;
            case State.Die:
                SetAnimStateMap("Die");
                OnDeadStateActivate();
                break;
            case State.CC:
                SetAnimStateMap("HardCC");
                
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
        CreateCCEffectByType(effect);

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

    // 상태맵 초기화
    private void InitStateMap()
    {
        stateMap.Add("Idle", false);
        stateMap.Add("Moving", false);
        stateMap.Add("Fire", false);
        stateMap.Add("PostDelay", false);
        stateMap.Add("HardCC", false);
        stateMap.Add("Skill_0", false);
        stateMap.Add("Skill_1", false);
        stateMap.Add("Skill_2", false);
        stateMap.Add("Die", false);
    }

    // 상태맵에서 원하는 상태만 True로 전환
    private void SetAnimStateMap(string stateName)
    {
        // Set True할 상태 먼저 Set.
        stateMap[stateName] = true;
        robotAnimator.SetBool(stateName, true);

        // 나머지는 False 처리
        foreach (var key in stateMap.Keys.ToList())
        {
            if (key != stateName)
            {
                stateMap[key] = false;
                robotAnimator.SetBool(key, false);
            }
        }
    }

    // CC기의 Type에 따라 이펙트 생성
    private void CreateCCEffectByType(SkillEffect effect)
    {
        switch (effect.ccType)
        {
            case CCType.Stun:
                GameObject go = Instantiate(stun, transform.position, Quaternion.identity);
                // 스턴 스킬의 지속시간을 life time으로 지정.
                go.GetComponent<MagicalFX._FX_LifeTime>().LifeTime = effect.duration;
                break;
            //case CCType.Curse:
            //    break;
        }
    }
}
