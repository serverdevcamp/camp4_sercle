using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    public enum State { Idle, Move, Attack, CC, Die }

    [Header("Robot Info")]
    [SerializeField] private int index;
    [SerializeField] protected Status status;
    [SerializeField] private int campNum;
    [SerializeField] private List<Vector3> destinations;
    [SerializeField] private int destFlag;
    private bool isDead;

    [Header("Attack Info")]
    [SerializeField] private State state;
    [SerializeField] private Robot target = null;
    [SerializeField] private LayerMask contactLayer;
    [SerializeField] private Attack attack;

    [Header("Effects")]
    [SerializeField] private List<GameObject> hitEffects;

    [SerializeField] private GameObject muzzle;

    private NavMeshAgent agent;

    public int Index { get { return index; } }
    public Status GetStatus { get { return status; } }
    public int CampNum { get { return campNum; } }
    public State GetState { get { return state; } }
    public Attack MyAttack { get { return attack; } }

    // 로봇 애니메이터
    private Animator robotAnimator;

    // 로봇 상태 - 부울 딕셔너리
    private Dictionary<string, bool> stateMap = new Dictionary<string, bool>();

    protected virtual void Start()
    {
        robotAnimator = GetComponent<Animator>();
        InitStateMap();
    }

    protected virtual void Update()
    {
        FindNearestTarget();
        StateMachine();
    }

    public void InitialSetting(int index, int campNum, List<Vector3> destinations)
    {
        this.index = index;
        this.campNum = campNum;
        this.destinations = destinations;
        state = State.Idle;
        status.ChangeStatTo(StatusType.CHP, status.MHP);

        if (campNum == 1) destFlag = 1;
        else destFlag = destinations.Count - 2;

        agent = GetComponent<NavMeshAgent>();
        agent.destination = destinations[destFlag];

        ShowMuzzleEffect(false);
    }

    private void FindNearestTarget()
    {
        target = null;

        Collider[] colls = Physics.OverlapSphere(transform.position, attack.range, contactLayer);

        float nearestDis = 9999999;

        foreach (Collider coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            if (campNum == coll.transform.GetComponent<Robot>().CampNum) continue;

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
                if (agent.remainingDistance <= 1)
                {
                    if (campNum == 1 && destFlag < destinations.Count - 1)
                    {
                        destFlag += 1;
                    }
                    else if(campNum == 2 && destFlag > 0)
                    {
                        destFlag -= 1;
                    }
                    agent.destination = destinations[destFlag];
                }
                break;
            case State.Attack:
                SetAnimStateMap("Skill_0");
                agent.speed = 0;
                AttackActivate();
                break;
            case State.Die:
                if (!isDead)
                {
                    isDead = true;
                    SetAnimStateMap("Die_" + Random.Range(0, 3).ToString());
                    OnDeadStateActivate();
                }
                break;
            case State.CC:
                SetAnimStateMap("HardCC");
                ShowMuzzleEffect(false);
                break;
            default:
                break;
        }
    }

    private void AttackActivate()
    {
        if (CampNum != GameManager.instance.MyCampNum) return;

        Vector3 dir = target.transform.position - transform.position;
        StartCoroutine(attack.Use(this, dir));
    }

    /// <summary>
    /// 서버에서 보내준 체력으로 이 로봇의 체력을 동기화 시킵니다.
    /// </summary>
    /// <param name="serverHP">서버에서 보내준 체력</param>
    public void Synchronize(float serverHP)
    {
        GetStatus.ChangeStatTo(StatusType.CHP, serverHP);
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
        else
        {
            if (effect.statusType != StatusType.CHP)
            {
                status.ChangeStat(effect.statusType, effect.amount);
            }
        }
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
        ShowMuzzleEffect(false);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        List<Canvas> canvasList = new List<Canvas>(transform.GetComponentsInChildren<Canvas>());
        for (int i = 0; i < canvasList.Count; i++)
        {
            canvasList[i].enabled = false;
        }
        //enabled = false;
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
        stateMap.Add("Die_0", false);
        stateMap.Add("Die_1", false);
        stateMap.Add("Die_2", false);
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

    // 피격된 skill number에 따라 적절한 이펙트 생성
    public void ShowHitEffect(int skillNumber, SkillEffect effect)
    {
        if (skillNumber - 8 < 0) return;

        GameObject go = Instantiate(hitEffects[skillNumber - 8], transform.position, Quaternion.identity);

        if (go != null)
            // 스턴 스킬의 지속시간을 life time으로 지정.
            go.GetComponent<MagicalFX.FX_LifeTime>().LifeTime = effect.duration;
    }

    // Muzzle 이펙트 활성화
    public void ShowMuzzleEffect(bool flag)
    {
        // Muzzle ON
        if (flag)
        {
            muzzle.SetActive(true);
        }
        else
        {
            muzzle.SetActive(false);
        }
    }

}
