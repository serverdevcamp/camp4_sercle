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

        GetHeroList();
    }

    private void Start()
    {
        robotManager = GetComponentInChildren<RobotManager>();
        indicateManager = GetComponentInChildren<IndicateManager>();

        if (GameObject.Find("DataObject").GetComponent<UserInfo>().userData.playerCamp == 1)
        {
            is1P = true;
        }
        else
        {
            is1P = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            indicateManager.ActivateSkillIndicator(myHeroes[0]);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            indicateManager.ActivateSkillIndicator(myHeroes[1]);
        }
    }

    private void GetHeroList()
    {
        // 히어로 리스트를 가져오세요.
        if (myHeroes.Count == 0) Debug.LogError("히어로 리스트를 가져와라!!!!");
    }

    // 2020 02 07 목적지와 현 위치의 거리를 보정한 원격 캐릭터의 속도 계산.
    public float SetInterpolatedSpeed(int index, Vector3 destination)
    {
        //float prevTime = Vector3.Distance(enemyCharacters[index].GetComponent<Transform>().position, destination) / enemyCharacters[index].status.SPD;
        //return (enemyCharacters[index].status.SPD) * (prevTime + SyncManager.instance.GetAvgRemoteRtt()) / prevTime;
        return 0;
    }

    public void FireRobotProjectile(int index, Vector3 dir)
    {
        robotManager.MyRobotFire(index, dir);
        SkillManager.instance.SendLocalSkillInfo(true, index, dir);
    }

    public void UseLocalSkill(int index, Vector3 pos, Vector3? dir = null)
    {
        myHeroes[index].UseSkill(pos, dir);
        //SkillManager.instance.SendLocalSkillInfo(false, index, pos, dir);
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

    public Hero GetMyHero(int i)
    {
        return myHeroes[i];
    }

    public int GetMyHeroCount()
    {
        return myHeroes.Count;
    }
}
