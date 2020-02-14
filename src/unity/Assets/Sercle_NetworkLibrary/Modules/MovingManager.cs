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

    // 원격 캐릭터의 agent의 속도
    private float[] remoteAgentSpeed = new float[3];

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
        //Debug.Log("전송전송 " + movingData);
        // 생성자로 데이터에 패킷을 연결
        MovingPacket packet = new MovingPacket(movingData);
        // UDP 전송
        // networkManager.SendUnreliable<MovingData>(packet);
        networkManager.SendReliable<MovingData>(packet);
    }

    // 이동 정보 패킷 획득 함수
    public void OnReceiveMovingPacket(PacketId id, byte[] data)
    {
        MovingPacket packet = new MovingPacket(data);   //바이트 데이터 역직렬
        MovingData moving = packet.GetPacket();
        // Debug.Log(moving + " 수신완료(이동)");

        Vector3 destination = new Vector3(moving.destX, moving.destY, moving.destZ);

        // 2020 02 01 상대 단말에서 상대의 로컬 캐릭터가 이동했을 때 송신한 정보를 수신한 것이므로 내 단말에서 리모트 캐릭터를 이동시킨다.
        //GameManager.instance.MoveEnemyCharacter(moving.index, destination);
        //// 2020 02 07 상대의 캐릭터의 이동속도 보정값을 저장.
        //remoteAgentSpeed[moving.index] = GameManager.instance.SetInterpolatedSpeed(moving.index, destination);
    }

    // rtt가 고려된 보정된 속도를 반환
    public float GetInterpolatedSpeed(int index)
    {
        return remoteAgentSpeed[index];
    }
}
