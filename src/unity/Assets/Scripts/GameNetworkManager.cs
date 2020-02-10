using UnityEngine;
using System.Collections;

public class GameNetworkManager : MonoBehaviour
{
    private string address = "10.99.13.48";
    private int port = 1000;
    public UserInfo userInfo;
    private TransportTCP socket;
    private NetworkManager networkManager;
    public bool userInfoFlag = false;
    // Use this for initialization
    void Start()
    {
        networkManager = transform.parent.GetComponent<NetworkManager>();
        userInfo = GameObject.Find("UserInfoObject").GetComponent<UserInfo>();
        socket = GetComponent<TransportTCP>();
        socket.Connect(address, port);
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

}
