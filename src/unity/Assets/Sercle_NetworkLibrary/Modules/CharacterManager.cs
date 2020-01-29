/*  
 * 캐릭터 관리자
 * 
 * 모든 캐릭터의 최신 정보를 송/수신 후 저장.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private GameObject[] players = new GameObject[3];
    [SerializeField]
    private CharacterData[] localPlayers = new CharacterData[3];
    [SerializeField]
    private CharacterData[] remotePlayers = new CharacterData[3];

    private NetworkManager networkManager;

    void Start()
    {
        // 네트워크 매니저 참조
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        // players[0] = gameobject,find("player0").getcomponent(characterData);

        // 캐릭터 정보 수신함수 등록
        networkManager.RegisterReceiveNotification(PacketId.CharacterData, OnReceiveCharacterPacket);

        // 더미 
        CreateDummy(localPlayers);
        CreateDummy(remotePlayers);
        remotePlayers[0].cc = -1;
        remotePlayers[1].cc = -1;
        remotePlayers[2].cc = -1;

        // 정기적으로 로컬 캐릭터 데이터를 서버로 보내는 코루틴 실행
        StartCoroutine(SendLocalCharacterInfo());
    }

    void CreateDummy(CharacterData[] player)
    {
        for (int i = 0; i < 3; i++)
        {
            player[i].playerIndex = i + 1;
            player[i].mhp = Random.Range(50, 100);
            player[i].chp = Random.Range(50, 100);
            player[i].spd = Random.Range(50, 100);
            player[i].atk = Random.Range(50, 100);
            player[i].def = Random.Range(50, 100);
            player[i].crt = Random.Range(50, 100);
            player[i].ddg = Random.Range(50, 100);
            player[i].cc = Random.Range(50, 100);
        }
    }


    void Update()
    {
        
    }

    private void LateUpdate()
    {
        UpdateLocalCharacterInfo();


    }
    
    // local character info를 프레임당 최신화
    private void UpdateLocalCharacterInfo()
    {
        for(int i = 0; i < 3; i++)
        {
            // localPlayers[i] = players[i].GetComponent<CharacterData>();
        }
    }


    // 주기적으로 local character data를 송신한다.
    private IEnumerator SendLocalCharacterInfo()
    {
        
        // while(newtworkmanager.isconnected())
        while (true)
        {
            for(int i = 0; i < 3; i++)
            {
                CharacterPacket packet = new CharacterPacket(localPlayers[i]);
                // UDP로 전송
                networkManager.SendUnreliable<CharacterData>(packet);
                // TCP로 전송
                //networkManager.SendReliable<CharacterData>(packet);
            }
            //Debug.Log("캐릭터 데이터 송신 완료");

            // 0.2초마다 데이터 송신
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 가장 최신의 remote character 정보 얻는다.
    public CharacterData GetRemoteCharacterInfo(int index)
    {
        return remotePlayers[index];
    }

    // remote character 정보 최신화한다.
    public void SetRemoteCharacterInfo(int index, CharacterData data)
    {
        remotePlayers[index] = data;
    }

    // 캐릭터 정보 패킷 획득 함수
    public void OnReceiveCharacterPacket(PacketId id, byte[] data)
    {
        CharacterPacket packet = new CharacterPacket(data);
        CharacterData character = packet.GetPacket();
        // Debug.Log(character.ToString() + " 수신완료.");
        //remotePlayers[character.playerIndex] = character;
    }

}
