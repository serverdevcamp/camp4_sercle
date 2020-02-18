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
    private int myCampNum;

    public int MyCampNum { get { return myCampNum; } }
    public int EnemyCampNum { get { return myCampNum == 1 ? 2 : 1; } }

    public Text tempWinnerText;

    // 양 단말 모두 게임씬으로 넘어와서 게임을 시작해도 되는지 판단하는 변수. readyToStart가 false면 일시정지, true면 정지 해제.
    [SerializeField]
    private bool readyToStart;

    // 각 단말이 게임씬으로 왔는지 체크하는 배열
    private bool[] enterGameScene = new bool[2];

    // 네트워크 매니저
    private NetworkManager networkManager;

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
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        myCampNum = GameObject.Find("DataObject").GetComponent<UserInfo>().userData.playerCamp;

        // 네트워크 매니저에 게임 종료, 시작 패킷 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.GameFinish, OnReceiveGameFinishPacket);
        networkManager.RegisterReceiveNotification(PacketId.GameStart, OnReceiveGameStartPacket);

        // 게임 시작할 준비가 되었다는 패킷 송신
        SendReadyToStartGamePacket();

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
        if (campNum == myCampNum)           // 로컬일 경우
        {
            if (isRobot) robotManager.MyRobotFire(index, pos, dir);
            else myHeroes[index].UseSkill(pos, dir);
        }
        else     // 리모트일 경우
        {
            if (isRobot) robotManager.EnemyRobotFire(index, pos, dir);
            else enemyHeroes[index].UseSkill(pos, dir);
        }
    }

    public void RequestSkillEffect(Robot target, SkillEffect effect)
    {
        int tCampNum = target.CampNum;
        int tIndex = target.Index;
        int statusType = (int)effect.statusType;
        int ccType = (int)effect.ccType;
        float amount = effect.amount;
        float duration = effect.duration;

        SkillManager.instance.SendLocalHitInfo(tCampNum, tIndex, statusType, ccType, amount, duration);
    }

    /// <summary>
    /// 서버로부터 전송받은 스킬의 효과를 대상에게 적용하도록 하는 함수
    /// </summary>
    /// <param name="tCampNum">타겟의 캠프 번호</param>
    /// <param name="tIndex">타겟이 되는 로봇의 번호</param>
    /// <param name="statusType">효과가 적용되는 스테이터스 타입</param>
    /// <param name="ccType">효과에 담긴 CC 타입</param>
    /// <param name="amount">변화할 스테이터스의 양</param>
    /// <param name="duration">변화할 시간. 0이면 무제한</param>
    public void ApplySkillEffect(int tCampNum, int tIndex, int statusType, int ccType, float amount, float duration)
    {
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

    // 양 클라이언트에서 동일한 타이밍으로 게임 시작을 위한, 게임시작 패킷 수신 함수
    public void OnReceiveGameStartPacket(PacketId id, byte[] data)
    {

        GameStartPacket packet = new GameStartPacket(data);
        GameStartData startData = packet.GetPacket();

        enterGameScene[startData.campNumber - 1] = true;

        bool check = false;
        
        for(int i = 0; i < 2; i++)
        {
           if(enterGameScene[i] == false)
            {
                Debug.Log(i.ToString() + " 진영에서 아직 데이터 안보냄.");
                check = true;
            }
        }

        if(check == false)
        {
            // 양 단말 모두 준비가 되었으므로 게임 시작
            Debug.Log("양 단말의 게임 시작 패킷 수신을 완료했으므로, 일시중지 해제하고 게임 시작");
            readyToStart = true;
        }
    }

    // 게임 씬으로 진입했을 때, 즉 게임 시작을 위한 준비가 모두 끝났을 때 서버에 준비 되었다는 정보를 송신
    public void SendReadyToStartGamePacket()
    {
        GameStartData data = new GameStartData();
        data.campNumber = MyCampNum;
        networkManager.SendReliable<GameStartData>(new GameStartPacket(data));
        Debug.Log(myCampNum.ToString() + " 게임씬 입장 데이터 전송 완료");
    }
}
