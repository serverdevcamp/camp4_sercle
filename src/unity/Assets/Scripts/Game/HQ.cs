/*
 * HQ가 파괴되었을 시, 파괴된 측이 패배.
 * 
 * 두 진영중 하나가 파괴되었을 때, 서버에 게임 종료 패킷을 보낸다.
 * 서버는 양 진영에게 게임 종료 패킷을 송신한다.
 * 
 * 수신한 양 진영은 다음씬 (or 데이터 수집) 진행한다.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : Robot
{
    protected override void Start()
    {
        status.ChangeStatTo(StatusType.CHP, status.MHP);
    }

    protected override void Update()
    {
        if (status.CHP <= 0)
        {
            GameFinishData data = new GameFinishData();

            if (CampNum == 1)
            {
                Debug.Log("1P의 패배!");
                data.winnerCamp = 2;    // 참고 : if GetComponent<UserInfo>().userData.playerCamp == 1 then is1p = true.

            }
            else
            {
                Debug.Log("2P의 패배!");
                data.winnerCamp = 1;
            }
            
            GameFinishPacket packet = new GameFinishPacket(data);
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().SendReliable<GameFinishData>(packet);

            Destroy(gameObject);
        }
    }
}
