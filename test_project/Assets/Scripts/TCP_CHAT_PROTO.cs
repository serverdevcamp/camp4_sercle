using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TCP_CHAT_PROTO : MonoBehaviour
{

    public TransportTCP tcp;

    public NetworkManager network;
    public List<float> avg = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        tcp = GetComponent<TransportTCP>();
        network = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        byte[] buffer = new byte[1024];
        int recvSize = tcp.Receive(ref buffer, buffer.Length);
        if (recvSize > 0)
        {
            string msg = System.Text.Encoding.UTF8.GetString(buffer);
            TimeSpan elapsedTime = new TimeSpan(DateTime.UtcNow.Ticks - long.Parse(msg));
            Debug.Log("RTT is : " + elapsedTime.Milliseconds);
        }
    }

    public void Btn()
    {
        if(!tcp.IsConnected())
            tcp.Connect("127.0.0.1", 3098);
        else
        {
            //string msg = Time.time.ToString();
            string msg = DateTime.UtcNow.Ticks.ToString();
            //msg = Time.
      
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
            tcp.Send(buffer, buffer.Length);
      
            Debug.Log("Send data : " + msg);
        }
    }
}
