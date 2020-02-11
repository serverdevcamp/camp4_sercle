using UnityEngine;
using System.Collections;
[SerializeField]
public class UserInfo : MonoBehaviour
{
    public UserData userData;
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
