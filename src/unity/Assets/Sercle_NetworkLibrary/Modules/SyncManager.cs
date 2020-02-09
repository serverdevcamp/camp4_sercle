/*
 * 로컬 단말의 RTT 측정용 패킷을 송신
 * 원격 단말의 RTT 패킷을 수신
 * 평균낸 원격 RTT를 원격 캐릭터의 스킬/이동 speed에 반영
 * 
 * 여기서의 RTT란, 상대가 정보를 보낸 시각과 현재 시각의 차이를 의미.
 * 
 * 이동, 스킬, 애니메이션 동기화까지 커버.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SyncManager : MonoBehaviour
{
    // 네트워크 매니저
    private NetworkManager networkManager;

    public static SyncManager instance;

    // 원격 단말의 RTT 계산
    [SerializeField]
    private List<int> rtt;

    // 틱 시작 기준
    DateTime beginDate = DateTime.Today;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        // 싱크 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.SyncData, OnReceiveSyncPacket);

        // 리스트 생성
        rtt = new List<int>();

        // 틱 송신 코루틴
        StartCoroutine(SendLocalSyncInfo());

    }

    // 이동 정보 패킷 획득 함수
    public void OnReceiveSyncPacket(PacketId id, byte[] data)
    {
        SyncPacket packet = new SyncPacket(data);
        SyncData sync = packet.GetPacket();
        Debug.Log(sync + " 수신완료(싱크).");

        TimeSpan elapsedSpan = new TimeSpan(DateTime.UtcNow.Ticks - sync.sendTime);

        if (rtt.Count >= 5)
        {
            rtt.RemoveAt(0);
        }

        rtt.Add(elapsedSpan.Milliseconds);

    }

    // 주기적으로 RTT 측정용 패킷을 송신한다.
    private IEnumerator SendLocalSyncInfo()
    {

        // while(newtworkmanager.isconnected())
        while (true)
        {
            SyncData data = new SyncData();
            data.sendTime = DateTime.UtcNow.Ticks;
            SyncPacket packet = new SyncPacket(data);

            networkManager.SendReliable<SyncData>(packet);
            // Debug.Log("Send Sync data " + data.ToString());
            // 2초마다 데이터 송신
            yield return new WaitForSeconds(2f);
        }
    }

    // 평균 RTT 시간
    public float GetAvgRemoteRtt()
    {
        int val = 0;
        for (int i = 0; i < rtt.Count; i++)
        {
            val += rtt[i];
        }
        // Debug.Log("VALUE1 : " + val / 5000f);
        // Debug.Log("VALUE2 : " + val * 0.0002f);
        // val이 40이면 0.04반환
        return val * 0.0002f;
    }

    // 최근 RTT 시간
    public float GetRecentRemoteRtt()
    {
        if (rtt.Count == 0)
        {
            return 0.001f;
        }
        return rtt[rtt.Count - 1] * 0.001f;
    }
}
