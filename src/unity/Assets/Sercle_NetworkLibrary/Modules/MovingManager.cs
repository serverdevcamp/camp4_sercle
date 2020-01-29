/*
 * 로컬 캐릭터의 이동 정보를 원격 클라이언트에게 전송
 * 원격 클라이언트가 보낸 이동 정보를 수신 후 원격 캐릭터의 이동에 반영
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingManager : MonoBehaviour
{
    public static MovingManager instance;

    // 네트워크 매니저
    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = transform.parent.GetComponent<NetworkManager>();
        // 이동 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.MovingData, OnReceiveMovingPacket);

    }

    private void FixedUpdate()
    {
        // Test
        if (Input.GetMouseButtonDown(0))  // 마우스가 클릭 되면
        {
            SendLocalMovingInfo(1, new Vector3(2, 3, 4));
        }
    }

    // 이동정보를 상대에게 전송하는 함수
    // 매개변수는 수정해도 상관 없음. 
    // 결과적으로 함수 내 MovingData의 멤버변수를 채워주기만 하면 됨.
    public void SendLocalMovingInfo(int index, Vector3 dest)
    {
        // 이동정보 데이터 생성 후 정보 입력
        MovingData movingData = new MovingData();
        movingData.index = index;
        movingData.destX = dest.x;
        movingData.destY = dest.y;
        movingData.destZ = dest.z;
        Debug.Log("전송 " + movingData);
        // 생성자로 데이터에 패킷을 연결
        MovingPacket packet = new MovingPacket(movingData);
        // UDP 전송
        // networkManager.SendUnreliable<MovingData>(packet);
        networkManager.SendReliable<MovingData>(packet);
    }

    // 이동 정보 패킷 획득 함수
    public void OnReceiveMovingPacket(PacketId id, byte[] data)
    {
        MovingPacket packet = new MovingPacket(data);
        MovingData moving = packet.GetPacket();
        Debug.Log(moving + " 수신완료(이동)");

        Vector3 destination = new Vector3(moving.destX, moving.destY, moving.destZ);
        GameManager.instance.MoveCharacter(moving.index, destination);
        
        // 수신 후 사용 예
        // navAgent(moving.index).destinaion(new Vector3(moving.destX, moving.destY, moving.dextZ);
    }
}
