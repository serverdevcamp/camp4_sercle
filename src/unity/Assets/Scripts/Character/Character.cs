using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterState { Idle, Move, Attack, Skill, Die }

public class Character : MonoBehaviour
{
    public int index;
    public Status status;
    public bool isFriend;

    [SerializeField] private CharacterState state;
    [SerializeField] private Character target = null;
    [SerializeField] private LayerMask contactLayer;

    [Header("Skills")]
    public bool usingSkill;
    public List<Skill> skills;

    [Header("Skill UI")]
    [SerializeField] private CircleRenderer skillRangeCircle;
    [SerializeField] private RectTransform skillDirRect;

    private NavMeshAgent agent;
    private bool isAttacking = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitialSetting();
    }

    private void Update()
    {
        FindNearestTarget();
        StateMachine();
    }

    private void InitialSetting()
    {
        status.ChangeStat(StatusType.CHP, status.MHP);
        for(int i = 0; i < skills.Count; i++)
        {
            skills[i].Initialize(i);
        }
    }

    private void FindNearestTarget()
    {
        target = null;

        Collider[] colls = Physics.OverlapSphere(transform.position, skills[0].range, contactLayer);

        float nearestDis = 9999999;

        foreach (Collider coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            // if (coll.gameObject.GetComponent<Character>().isFriend) continue;

            // 2020 02 01 이 캐릭터의 isFriend와 검출된 캐릭터의 isFriend가 같다면, isFriend의 T/F에 관계 없이 이 캐릭터의 아군이다. 아군 검출시 continue.
            // isFriend true인 애를 안공격하는 문제는 위의 if (coll.gameObject.GetComponent<Character>().isFriend) continue;문을 지우면 됨.
            if (isFriend == coll.transform.GetComponent<Character>().isFriend) continue;

            if (target == null || Vector3.Distance(coll.transform.position, transform.position) < nearestDis)
            {
                nearestDis = Vector3.Distance(coll.transform.position, transform.position);
                target = coll.GetComponent<Character>();
            }
        }
    }

    private void BasicAttackActivate()
    {
        if (isFriend == false) return;
        Vector3 dir = target.transform.position - transform.position;
        // 발사체 오브젝트 교체
        ChangeProjectile();
        StartCoroutine(skills[0].Use(this, dir.normalized));
    }

    public void UseSkill(int skillNum)
    {
        StartCoroutine(skills[skillNum].Use(this));
    }

    public void FireProjectile(int num, Vector3 dir)
    {
        StartCoroutine(skills[num].Fire(this, dir));        
    }

    private void StateMachine()
    {
        if (status.CHP <= 0)
        {
            state = CharacterState.Die;
        }
        else if (status.HardCC != HardCCType.None)
        {
            // CC기에 따른 행동
            // 현재는 Stun 밖에 없음
            agent.destination = transform.position;
        }
        else if (usingSkill)
        {
            state = CharacterState.Skill;
        }
        else if (state != CharacterState.Skill && agent.remainingDistance > agent.stoppingDistance)
        {
            state = CharacterState.Move;
        }
        else if ((state == CharacterState.Idle || state == CharacterState.Move) && target)
        {
            state = CharacterState.Attack;
        }
        else
        {
            state = CharacterState.Idle;
        }

        StateAction();
    }

    private void StateAction()
    {
        switch (state)
        {
            case CharacterState.Idle:
                agent.speed = 0;
                break;
            case CharacterState.Move:
                agent.speed = status.SPD;
                break;
            case CharacterState.Attack:
                BasicAttackActivate();
                break;
            case CharacterState.Skill:
                break;
            case CharacterState.Die:
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    private void OnMouseEnter()
    {
        //MouseCursor.instance.State = MouseState.Select;
    }

    /// <summary>
    /// 마우스가 이 캐릭터의 콜라이더를 클릭하면 불리는 함수.
    /// GameManager에게 자신이 클릭됐음을 전달.
    /// </summary>
    private void OnMouseDown()
    {
        if (isFriend == false) return;

        Debug.Log("You click " + name);
        GameManager.instance.ChangeCurrentCharacter(this);
    }

    private void OnMouseExit()
    {
        //MouseCursor.instance.State = MouseState.Idle;
    }

    public void SetDestination(Vector3 pos)
    {
        agent.destination = pos;
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
            // 알아서 이 effect를 적용시키시오!
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
    /// 해당 캐릭터를 선택/해제할 때 상태를 바꿔주는 함수
    /// </summary>
    /// <param name="choose">true면 선택되는 시점에 콜. false면 해제되는 시점에 콜.</param>
    public void ChooseToCurrent(bool choose)
    {
        if (choose)
        {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        }
        else
        {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
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

    // EffectController에서 CharacterState를 받아오게끔 하는 함수. 200130 작성
    public CharacterState GetCharacterState()
    {
        return state;
    }

    // 지속시간 있는 버프 스킬 발동시 투사체 변환
    private void ChangeProjectile() 
    {
        if (index == 1)
        {
            if (GetComponent<EffectController>().SkillState(2))
                skills[0].proj = GetComponent<EffectController>().projectilePrefabs[2].GetComponent<Projectile>();
            else
                skills[0].proj = GetComponent<EffectController>().projectilePrefabs[0].GetComponent<Projectile>();
        }
    }
}
