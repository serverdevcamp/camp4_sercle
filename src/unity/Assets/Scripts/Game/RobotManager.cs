using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotManager : MonoBehaviour
{
    [Header("Line Pos")]
    [SerializeField] private List<Transform> line1;
    [SerializeField] private List<Transform> line2;
    [SerializeField] private List<Transform> line3;

    [Header("Wave Info")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private int waveSize;
    [SerializeField] private float waveTerm;

    private List<GameObject> firstCampRobots = new List<GameObject>();
    private List<GameObject> secondCampRobots = new List<GameObject>();
    private int robotNum = 0;

    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    private void Start()
    {
        // 로봇 생성 신호 수신함수 등록.
        networkManager.RegisterReceiveNotification(PacketId.SpawnRobotsData, OnReceiveRobotSpawnPacket);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("엔터 누름");
            SendSpawnRobotData();
            //StartCoroutine(SpawnRobots());
        }
    }

    private IEnumerator SpawnRobots()
    {
        for (int i = 0; i < waveSize; i++)
        {
            //SpawnRobotPair(LinePos(line1));
            SpawnRobotPair(LinePos(line2));
            //SpawnRobotPair(LinePos(line3));

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnRobotPair(List<Vector3> linePos)
    {
        int count = linePos.Count;

        GameObject firstCampRobot = Instantiate(robotPrefab, linePos[0], Quaternion.identity);
        firstCampRobot.GetComponent<Robot>().InitialSetting(robotNum, 1, linePos);
        firstCampRobots.Add(firstCampRobot);

        GameObject secondCampRobot = Instantiate(robotPrefab, linePos[count - 1], Quaternion.identity);
        secondCampRobot.GetComponent<Robot>().InitialSetting(robotNum, 2, linePos);
        secondCampRobots.Add(secondCampRobot);
        
        robotNum += 1;
    }

    private List<Vector3> LinePos(List<Transform> line)
    {
        List<Vector3> linePos = new List<Vector3>();
        for (int i = 0; i < line.Count; i++)
        {
            linePos.Add(line[i].position);
        }

        return linePos;
    }

    public void FirstCampRobotFire(int index, Vector3 pos, Vector3 dir)
    {
        Robot caster = firstCampRobots[index].GetComponent<Robot>();
        caster.transform.position = pos;
        StartCoroutine(caster.MyAttack.Fire(caster, dir));
    }

    public void SecondCampRobotFire(int index, Vector3 pos, Vector3 dir)
    {
        Robot caster = secondCampRobots[index].GetComponent<Robot>();
        caster.transform.position = pos;
        StartCoroutine(caster.MyAttack.Fire(caster, dir));
    }

    // 로봇 스폰 생성 신호 송신(테스트용)
    public void SendSpawnRobotData()
    {
        SpawnRobotData data = new SpawnRobotData();

        data.trash = GameManager.instance.MyCampNum;
        
        SpawnRobotPacket packet = new SpawnRobotPacket(data);


        
        networkManager.SendReliable<SpawnRobotData>(packet);
        Debug.Log("미니언 만들라고 신호 보냈음.");
    }
    
    // 로봇 생성 신호를 서버로부터 수신하는 함수
    public void OnReceiveRobotSpawnPacket(PacketId id, byte[] data)
    {
        // 서버는, 주기적으로 Packet ID만 붙혀서 송신한다.
        // 로봇 스폰 코드
        SpawnRobotPacket packet = new SpawnRobotPacket(data);
        SpawnRobotData spawnData = packet.GetPacket();

        Debug.Log(spawnData.trash + " 가 엔터 눌렀음.");

        StartCoroutine(SpawnRobots());
    }

    public Robot MyRobot(int i)
    {
        return firstCampRobots[i].GetComponent<Robot>();
    }

    public Robot EnemyRobot(int i)
    {
        return secondCampRobots[i].GetComponent<Robot>();
    }
}
