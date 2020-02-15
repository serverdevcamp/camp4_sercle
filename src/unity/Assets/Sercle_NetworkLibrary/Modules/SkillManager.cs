using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // 네트워크 매니저
    private NetworkManager networkManager;

    // 스킬 선택 씬에서 선택한 스킬 번호 리스트
    public List<int> mySkills = new List<int>();
    public List<int> enemySkills = new List<int>();

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
        // 스킬 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.SkillData, OnReceiveSkillPacket);
        // 스킬 선택 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.SelectedSkillData, OnReceiveSelectedSkillPacket);
    }

    /// <summary>
    /// 스킬 정보를 상대방에게 전송하는 코드
    /// </summary>
    /// <param name="isRobot">로봇이 사용한 공격인가?</param>
    /// <param name="index">로봇/영웅의 번호</param>
    /// <param name="dir">방향</param>
    public void SendLocalSkillInfo(bool isRobot, int index, Vector3 dir)
    {
        // 이동정보 데이터 생성 후 정보 입력
        SkillData skillData = new SkillData();
        skillData.isRobot = isRobot;
        skillData.index = index;
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
        GameManager.instance.FireRemoteProjectile(skill.isRobot, skill.index, dir);
    }

    // 스킬 선택 씬에서 선택했던 스킬 번호 획득 함수
    public void OnReceiveSelectedSkillPacket(PacketId id, byte[] data)
    {
        SelectedSkillPacket packet = new SelectedSkillPacket(data);
        SelectedSkillData skillInfo = packet.GetPacket();

        // 스킬 번호 리스트에 추가
        if(skillInfo.userId == MatchingManager.instance.myInfo)
        {
            mySkills.Add(skillInfo.skillQ);
            mySkills.Add(skillInfo.skillW);
            mySkills.Add(skillInfo.skillE);
        }
        else if(skillInfo.userId == MatchingManager.instance.opponentInfo)
        {
            enemySkills.Add(skillInfo.skillQ);
            enemySkills.Add(skillInfo.skillW);
            enemySkills.Add(skillInfo.skillE);
        }
        else
        {
            Debug.Log("Selected Skills의 user id와 일치하는 id 없음.");
        }
    }
}
