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
        // Debug.Log("전송 " + skillData);
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
        // Debug.Log(skill + " 수신완료(스킬).");

        Vector3 dir = new Vector3(skill.dirX, skill.dirY, skill.dirZ);

        // 2020 02 01상대 단말의 로컬 캐릭터가 스킬 사용했다는 정보를 내 단말에서 수신 한것이므로, 내 단말의 상대 캐릭터가 스킬사용했다고 해줘야함.
        GameManager.instance.FireRemoteProjectile(skill.index, skill.num, dir);
    }
}
