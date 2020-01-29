/*
 * 로컬 캐릭터의 스킬 사용 정보를 원격 클라이언트에게 전송
 * 원격 클라이언트가 보낸 스킬 정보를 수신 후 원격 캐릭터에 반영
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    
    // 네트워크 매니저
    private NetworkManager networkManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        // 이동 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.SkillData, OnReceiveSkillPacket);
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }


    // 스킬 정보를 상대에게 전송하는 함수
    public void SendLocalSkillInfo(SkillData data)
    {
        // 패킷 붙힘.
        SkillPacket packet = new SkillPacket(data);
        // UDP 전송
        //networkManager.SendUnreliable<SkillData>(packet);

        //TCP 전송
        networkManager.SendReliable<SkillData>(packet);

        Debug.Log("전송 " + data);
    }


    // 이동 정보 패킷 획득 함수
    public void OnReceiveSkillPacket(PacketId id, byte[] data)
    {
        SkillPacket packet = new SkillPacket(data);
        SkillData skill = packet.GetPacket();
        Debug.Log(skill + " 수신완료(스킬).");

        // 수신 후 사용 예
        // navAgent(moving.index).destinaion(new Vector3(moving.destX, moving.destY, moving.dextZ);
    }
}
