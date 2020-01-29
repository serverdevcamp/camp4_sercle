using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PT4_Network : MonoBehaviour
{

    [SerializeField]
    private bool isNetConnected;
   

    // Start is called before the first frame update
    void Start()
    {
        isNetConnected = false;
        // 일단 정지
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        



    }



    public void ConnectIP()
    {
        if (!isNetConnected)
        {
            Debug.Log("UDP 연결 버튼 클릭됨.");
            isNetConnected = GetComponent<TransportUDP>().Connect("127.0.0.1", 3098);
            if(isNetConnected)
                Time.timeScale = 1f;
        }
    }
}
