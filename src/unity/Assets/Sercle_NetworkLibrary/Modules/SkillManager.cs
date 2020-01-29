using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // 네트워크 매니저
    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = transform.parent.GetComponent<NetworkManager>();
        // 이동 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.SkillData, OnReceiveSkillPacket);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // 우클릭시
        if (Input.GetMouseButtonDown(1))
        {
            SkillData data = new SkillData();
            data.index = 1;
            data.duration = 1.5f;
            data.amount = 3;
            data.count = 2;
            data.types = new int[2];
            data.types[0] = (int)StatusType.ATK;
            data.types[1] = (int)StatusType.SPD;
            SendLocalSkillInfo(data);
        }*/
    }


    // 스킬 정보를 상대에게 전송하는 함수
    public void SendLocalSkillInfo(int index, int num, Vector3 dir)
    {
        // 이동정보 데이터 생성 후 정보 입력
        SkillData skillData = new SkillData();
        skillData.index = index;
        skillData.num = num;
        skillData.dirX = dir.x;
        skillData.dirY = dir.y;
        skillData.dirZ = dir.z;
        Debug.Log("전송 " + skillData);
        // 생성자로 데이터에 패킷을 연결
        SkillPacket packet = new SkillPacket(skillData);
        // UDP 전송
        //networkManager.SendUnreliable<SkillData>(packet);
        networkManager.SendReliable<SkillData>(packet);
    }


    // 이동 정보 패킷 획득 함수
    public void OnReceiveSkillPacket(PacketId id, byte[] data)
    {
        SkillPacket packet = new SkillPacket(data);
        SkillData skill = packet.GetPacket();
        Debug.Log(skill + " 수신완료(스킬).");

        Vector3 dir = new Vector3(skill.dirX, skill.dirY, skill.dirZ);
        GameManager.instance.FireProjectile(skill.index, skill.num, dir);

        // 수신 후 사용 예
        // navAgent(moving.index).destinaion(new Vector3(moving.destX, moving.destY, moving.dextZ);
    }
}
