using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;
// 임시 승리 텍스트를 띄우기 위한 UI 사용
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<Hero> firstCampHeroes = new List<Hero>();
    [SerializeField] private List<Hero> secondCampHeroes = new List<Hero>();
    [SerializeField] private GameObject sampleHero;

    private RobotManager robotManager;
    private IndicateManager indicateManager;
    private int myCampNum;

    public int MyCampNum { get { return myCampNum; } }
    public int EnemyCampNum { get { return myCampNum == 1 ? 2 : 1; } }

    [Header("DataCount")]
    [SerializeField] private int killCount; // 죽인 로봇의 개수
    [SerializeField] private float damageCount; // 상대 로봇에게 가한 데미지 총량
    

    public Hero MyHero(int i)
    {
        if (MyCampNum == 1) return firstCampHeroes[i];
        else return secondCampHeroes[i];
    }

    public Hero EnemyHero(int i)
    {
        if (EnemyCampNum == 1) return firstCampHeroes[i];
        else return secondCampHeroes[i];
    }

    // 양 단말 모두 게임씬으로 넘어와서 게임을 시작해도 되는지 판단하는 변수. readyToStart가 false면 일시정지, true면 정지 해제.
    [SerializeField] private bool readyToStart;
    [SerializeField] private GameObject loadingCanvasPrefab;
    private GameObject loadingCanvas;

    // 각 단말이 게임씬으로 왔는지 체크하는 배열
    private bool[] enterGameScene = new bool[2];

    // 네트워크 매니저
    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        robotManager = GetComponentInChildren<RobotManager>();
        indicateManager = GetComponentInChildren<IndicateManager>();

        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        myCampNum = GameObject.Find("DataObject").GetComponent<UserInfo>().userData.playerCamp;

        GetHeroList();
    }

    private void Start()
    {
        // 네트워크 매니저에 게임 종료, 시작 패킷 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.GameFinish, OnReceiveGameFinishPacket);
        networkManager.RegisterReceiveNotification(PacketId.GameStart, OnReceiveGameStartPacket);

        // 게임 시작할 준비가 되었다는 패킷 송신
        SendReadyToStartGamePacket();

        loadingCanvas = Instantiate(loadingCanvasPrefab);

        // BGM 실행.
        SoundManager.instance.PlayBGM("Game_BGM", 0.2f);

        killCount = 0;
        damageCount = 0;
    }

    private void Update()
    {
        if (loadingCanvas.activeInHierarchy == true) return;
        
        if (Input.GetKeyDown(KeyCode.Q)) indicateManager.ActivateSkillIndicator(MyHero(0));
        else if (Input.GetKeyDown(KeyCode.W)) indicateManager.ActivateSkillIndicator(MyHero(1));
        else if (Input.GetKeyDown(KeyCode.E)) indicateManager.ActivateSkillIndicator(MyHero(2));
    }

    // SkillManager에 있는 스킬 가져오는 함수.
    private void GetHeroList()
    {
        if (SkillManager.instance.firstCampSkills.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 initPos = new Vector3(999 + i * 10, 10, 999);
                GameObject tmpHero = Instantiate(sampleHero, initPos, Quaternion.identity);
                tmpHero.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                tmpHero.GetComponent<Hero>().Index = i;
                tmpHero.GetComponent<Hero>().InitialPos = initPos;
                tmpHero.GetComponent<Hero>().Initialize(SkillManager.instance.firstCampSkills[i]);
                firstCampHeroes.Add(tmpHero.GetComponent<Hero>());

            }
        }
        if (SkillManager.instance.secondCampSkills.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 initPos = new Vector3(999 - (i + 1) * 10, 10, 999);
                GameObject tmpHero = Instantiate(sampleHero, initPos, Quaternion.identity);
                tmpHero.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                tmpHero.GetComponent<Hero>().Index = i;
                tmpHero.GetComponent<Hero>().InitialPos = initPos;
                tmpHero.GetComponent<Hero>().Initialize(SkillManager.instance.secondCampSkills[i]);
                secondCampHeroes.Add(tmpHero.GetComponent<Hero>());
            }
        }
    }

    public void RequestFire(int campNum, bool isRobot, int index, Vector3 pos, Vector3 dir)
    {
        SkillManager.instance.SendLocalSkillInfo(campNum, isRobot, index, pos, dir);
    }

    public void ApplyFire(int campNum, bool isRobot, int index, Vector3 pos, Vector3 dir)
    {
        if (campNum == 1)
        {
            if (isRobot) robotManager.FirstCampRobotFire(index, pos, dir);
            else firstCampHeroes[index].UseSkill(pos, dir);
        }
        else
        {
            if (isRobot) robotManager.SecondCampRobotFire(index, pos, dir);
            else secondCampHeroes[index].UseSkill(pos, dir);
        }
    }

    public void RequestSkillEffect(Robot target, SkillEffect effect)
    {
        int tCampNum = target.CampNum;
        int tIndex = target.Index;
        int statusType = (int)effect.statusType;
        int ccType = (int)effect.ccType;
        int amount = effect.amount;
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
    public void ApplySkillEffect(int tCampNum, int tIndex, int statusType, int ccType, int amount, float duration, float chp)
    {
        Robot target;
        if (tCampNum == 1) target = robotManager.FirstCampRobot(tIndex);
        else target = robotManager.SecondCampRobot(tIndex);

        SkillEffect effect = new SkillEffect((StatusType)statusType, (CCType)ccType, amount, duration);

        if (target == null)
        {
            Debug.Log("해당하는 타겟을 찾을 수 없습니다. 스킬 효과 적용을 무시합니다." + System.Environment.NewLine
                + "타겟 정보 : " + tCampNum + "P의 " + tIndex + "번째 로봇" + System.Environment.NewLine
                + "스킬 정보 : " + (StatusType)statusType + "스탯, " + (CCType)ccType + "CC 기, " + amount + "만큼 " + duration + "초 동안 변화");
            return;
        }
        target.Synchronize(chp);
        target.Apply(effect);

        // 상대 로봇에게 가한 damage를 게임매니저에 저장.
        if ((StatusType)statusType == StatusType.CHP && tCampNum != MyCampNum)
        {
            if (amount < 0) IncDamageCount(-amount);
        }
    }

    // HQ가 파괴되었다는 패킷 수신 함수
    public void OnReceiveGameFinishPacket(PacketId id, byte[] data)
    {
        GameFinishPacket packet = new GameFinishPacket(data);
        GameFinishData winnerData = packet.GetPacket();

        HTTPManager httpManager = new HTTPManager();
        UserInfo userInfo = GameObject.Find("DataObject").GetComponent<UserInfo>();

        bool win = userInfo.userData.playerCamp == winnerData.winnerCamp;

        httpManager.UpdateUserWinReq(userInfo.userData.id, win);

        UIManager.instance.ActivateGameEnd(win);

        // 서버에 보낼 게임 정보 업데이트
        userInfo.userPlayData.achievescore = 0;
        userInfo.userPlayData.victory = 0;
        userInfo.userPlayData.lose = 0;
        userInfo.userPlayData.death = 1;
        userInfo.userPlayData.imageid = 0;

        userInfo.userPlayData.kill = killCount;
        userInfo.userPlayData.damage = (int)damageCount;
        
        // Flask 서버에 업적 업데이트 및 점수 업데이트.
        httpManager.UpdateAchieveReq(userInfo.userData.id, userInfo.userPlayData);

        // n 초 뒤에 로비씬으로 이동.
        StartCoroutine(GoBackToLobby());
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
                Debug.Log((i + 1).ToString() + " 진영에서 아직 데이터 안보냄.");
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

    // 게임 종료시, 게임씬에서 로비로 복귀한다.
    private IEnumerator GoBackToLobby()
    {
        yield return new WaitForSeconds(5f);
        Destroy(GameObject.Find("NetworkManager").gameObject);
        MatchingManager.instance.MatchState = MatchingManager.MatchingState.Nothing;
        SceneManager.LoadScene("Lobby");
    }

    // KillCount증가
    public void IncKillCount()
    {
        killCount++;
    }

    // Demage Count 증가
    public void IncDamageCount(float amount)
    {
        damageCount += amount;
    }
}
