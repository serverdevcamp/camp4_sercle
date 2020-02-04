using UnityEngine;
using System.Collections;
[SerializeField]
public class UserInfo : MonoBehaviour
{
    public UserData userData;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
