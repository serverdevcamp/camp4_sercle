using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameNetworkManager : MonoBehaviour
{
    private string address = Info.IP;
    private int port = 1000;
    public UserInfo userInfo;
    private TransportTCP socket;
    private NetworkManager networkManager;
    public bool userInfoFlag = false;
    public bool clientID;
    // Use this for initialization
    void Start()
    {
        networkManager = transform.parent.GetComponent<NetworkManager>();
        userInfo = GameObject.Find("DataObject").GetComponent<UserInfo>();

        networkManager.RegisterReceiveNotification(PacketId.GameServerEnd,
            OnReceiveGameEndPacket);
    }

    // Update is called once per frame
    void Update()
    {
        if(userInfoFlag == false)
        {
            SendLocalGameJoin();
            userInfoFlag = true;
        }

    
    }
    //게임 시작 메세지 전
    public void SendLocalGameJoin()
    {
        GameJoinData gameJoinData = new GameJoinData();
        gameJoinData.id = int.Parse(userInfo.userData.id);
        gameJoinData.roomNum = userInfo.userData.roomNum;

        GameJoinPacket packet = new GameJoinPacket(gameJoinData);
        networkManager.SendReliable<GameJoinData>(packet);
    }

    public void SendLocalSkillSelect(SelectedSkillPacket packet)
    {
        networkManager.SendReliable<SelectedSkillData>(packet);
    }

    public void OnReceiveGameEndPacket(PacketId id, byte[] data)
    {
        GameEndPacket packet = new GameEndPacket(data);
        GameEndData packetData = packet.GetPacket();

        //상대방이 게임을 종료했을 시 -> 게임 승리로 간주
        if(packetData.request == GamePacketId.OpponentEnd)
        {
            networkManager.transportTCP.Disconnect();
            MatchingManager.instance.MatchState = MatchingManager.MatchingState.Nothing;
            SceneManager.LoadScene("Lobby");

           
        }
    }
}
