/*
 * 로컬 캐릭터의 이동 정보를 원격 클라이언트에게 전송
 * 원격 클라이언트가 보낸 이동 정보를 수신 후 원격 캐릭터의 이동에 반영
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingManager : MonoBehaviour
{

    // 네트워크 매니저
    private NetworkManager networkManager;


    // Start is called before the first frame update
    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        // 이동 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.MovingData, OnReceiveMovingPacket);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Test
        if (Input.GetMouseButtonDown(0))  // 마우스가 클릭 되면
        {
            Vector3 pos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float depth;
            plane.Raycast(ray, out depth);

            Vector3 worldPos = ray.origin + ray.direction * depth;

            SendLocalMovingInfo(1, new Vector3(worldPos.x, worldPos.y, worldPos.z));
            GameObject.Find("GameController").GetComponent<PT4_GameManager>().GameMovingUpdate(worldPos);
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
        //networkManager.SendUnreliable<MovingData>(packet);

        // TCP 전송
        networkManager.SendReliable<MovingData>(packet);
    }

    // 이동 정보 패킷 획득 함수
    public void OnReceiveMovingPacket(PacketId id, byte[] data)
    {
        MovingPacket packet = new MovingPacket(data);
        MovingData moving = packet.GetPacket();
        Debug.Log(moving + " 수신완료(마우스).");

        // 수신 후 사용 예
        // navAgent(moving.index).destinaion(new Vector3(moving.destX, moving.destY, moving.dextZ);

        GameObject.Find("GameController").GetComponent<PT4_GameManager>().RecvMovingMsg(new Vector3(moving.destX, moving.destY, moving.destZ));

    }
}
