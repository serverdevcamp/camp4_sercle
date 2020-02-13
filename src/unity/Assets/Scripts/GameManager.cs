using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<Hero> myHeroes = new List<Hero>();
    [SerializeField] private List<Hero> enemyHeroes = new List<Hero>();

    private RobotManager robotManager;
    private IndicateManager indicateManager;
    private bool is1P;

    public bool Is1P { get { return is1P; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        is1P = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().clientID;

        GetHeroList();
    }

    private void Start()
    {
        robotManager = GetComponentInChildren<RobotManager>();
        indicateManager = GetComponentInChildren<IndicateManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            indicateManager.ActivateSkillIndicator(myHeroes[0]);
        }
    }

    private void GetHeroList()
    {
        // 히어로 리스트를 가져오세요.
        if (myHeroes.Count == 0) Debug.LogError("히어로 리스트를 가져와라!!!!");
    }

    public Vector3? GetDirection(Character caster, ref bool isValid)
    {
        Vector3? dir = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 casterPos = caster.transform.position;
            Vector3 rawDir = hit.point - casterPos;

            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("마우스 위치 : " + hit.point + ", 시전자 위치 : " + casterPos + ", 계산된 방향 : " + rawDir);
                dir = rawDir.normalized;
                //caster.ShowSkillDirection(false);
            }
        }

        return dir;
    }

    // 2020 02 07 목적지와 현 위치의 거리를 보정한 원격 캐릭터의 속도 계산.
    //public float SetInterpolatedSpeed(int index, Vector3 destination)
    //{
    //    float prevTime = Vector3.Distance(enemyCharacters[index].GetComponent<Transform>().position, destination) / enemyCharacters[index].status.SPD;

    //    return (enemyCharacters[index].status.SPD) * (prevTime + SyncManager.instance.GetAvgRemoteRtt()) / prevTime;
    //}

    public void FireRobotProjectile(int index, Vector3 dir)
    {
        robotManager.MyRobotFire(index, dir);
        SkillManager.instance.SendLocalSkillInfo(true, index, dir);
    }

    public void UseLocalSkill(int index, Vector3 pos, Vector3? dir = null)
    {
        myHeroes[index].UseSkill(pos, dir);
        SkillManager.instance.SendLocalSkillInfo(false, index, pos, dir);
    }

    public void FireProjectile(int index, Vector3 pos, Vector3? dir = null)
    {
        // index 번째 myHero를 pos 위치에 instantiate한다.
        // dir 방향으로 스킬을 사용하도록 초기화
        // SkillManager에게 이 사실을 알린다.
        // SkillManager.instance.SendLocalSkillInfo(false, index, dir);
    }

    // 2020 02 01 원격 캐릭터가 투사체 발사하도록 한다.
    public void FireRemoteProjectile(bool isRobot, int index, Vector3 dir)
    {
        if(isRobot) robotManager.EnemyRobotFire(index, dir);
        //else enemyCharacters[index].FireProjectile(num, dir);
    }

    /// <summary>
    /// 계산된 효과를 target 캐릭터에게 적용하는 함수.
    /// 모든 공격, 효과는 이 함수를 거쳐가도록 할 예정.
    /// </summary>
    /// <param name="target">효과를 적용할 대상</param>
    /// <param name="effects">적용할 효과의 리스트</param>
    public void ApplySkill(Robot target, SkillEffect effect)
    {
        target.Apply(effect);
    }
}