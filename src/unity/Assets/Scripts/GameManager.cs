using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;
// 임시 승리 텍스트를 띄우기 위한 UI 사용
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<Hero> myHeroes = new List<Hero>();
    [SerializeField] private List<Hero> enemyHeroes = new List<Hero>();

    private RobotManager robotManager;
    private IndicateManager indicateManager;
    private bool is1P;

    public bool Is1P { get { return is1P; } }

    public Text tempWinnerText;

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

        // 네트워크 매니저에 게임 종료 패킷 수신함수 등록
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().RegisterReceiveNotification(PacketId.GameFinish, OnReceiveGameFinishPacket);

        // 임시 승리 텍스트 안보이게 함.
        tempWinnerText.text = "";
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

    public void RequestFire(int campNum, bool isRobot, int index, Vector3 pos, Vector3 dir)
    {
        SkillManager.instance.SendLocalSkillInfo(campNum, isRobot, index, pos, dir);
    }

    public void ApplyFire(int campNum, bool isRobot, int index, Vector3 pos, Vector3 dir)
    {
        int myCampNum = is1P ? 1 : 2;
        if (campNum == myCampNum)           // 로컬일 경우
        {
            if (isRobot) robotManager.MyRobotFire(index, pos, dir);
            else myHeroes[index].UseSkill(pos, dir);
        }
        else     // 리모트일 경우
        {
            if(isRobot) robotManager.EnemyRobotFire(index, pos, dir);
            else enemyHeroes[index].UseSkill(pos, dir);
        }
    }

    public void RequestSkillEffect(int tCampNum, int tIndex, int statusType, int ccType, float amount, float duration)
    {
        SkillManager.instance.SendLocalHitInfo(tCampNum, tIndex, statusType, ccType, amount, duration);
    }

    /// <summary>
    /// 서버로부터 전송받은 스킬의 효과를 대상에게 적용하도록 하는 함수
    /// </summary>
    /// <param name="tCampNum"></param>
    /// <param name="tIndex"></param>
    /// <param name="statusType"></param>
    /// <param name="ccType"></param>
    /// <param name="amount"></param>
    /// <param name="duration"></param>
    public void ApplySkillEffect(int tCampNum, int tIndex, int statusType, int ccType, float amount, float duration)
    {
        int myCampNum = is1P ? 1 : 2;
        Robot target;
        if (tCampNum == myCampNum) target = robotManager.MyRobot(tIndex);
        else target = robotManager.EnemyRobot(tIndex);

        SkillEffect effect = new SkillEffect((StatusType)statusType, (CCType)ccType, amount, duration);

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

    // HQ가 파괴되었다는 패킷 수신 함수
    public void OnReceiveGameFinishPacket(PacketId id, byte[] data)
    {
        GameFinishPacket packet = new GameFinishPacket(data);
        GameFinishData winnerData = packet.GetPacket();

        // 승리 진영과 자신의 진영이 일치할경우, 승리 판정.
        if (GameObject.Find("DataObject").GetComponent<UserInfo>().userData.playerCamp == winnerData.winnerCamp)
        {
            tempWinnerText.text = "THE WINNER IS : ME ^^";   
        }
        // 패배 판정
        else
        {
            tempWinnerText.text = "THE WINNER IS OPPONENT TT";
        }
    }

}
