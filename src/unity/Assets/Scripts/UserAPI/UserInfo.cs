using UnityEngine;
using System.Collections.Generic;

[SerializeField]
public class UserInfo : MonoBehaviour
{
    public UserData userData;
    public UserPlayData userPlayData;
    public List<UserAchieveData> userAchieveData;   //유저가 달성한 업적 리스
    public List<UserAchieveData> AchieveData;       //업적 리스트
    // Use this for initialization
    public HTTPManager hTTPManager;
    void Start()
    {
        hTTPManager = new HTTPManager();
        DontDestroyOnLoad(gameObject);

    }
    void OnApplicationQuit()
    {
        hTTPManager.DestroyUserCache(userData.email);
        Debug.Log("Application Quit");
    }
}
