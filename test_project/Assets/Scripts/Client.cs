using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
public class Client : MonoBehaviour
{
    public string address;

    public int port = 3098;

    public Socket listener;
    public Socket socket;

    public TransportTCP mysocket = new TransportTCP();

    // Start is called before the first frame update
    void Start()
    {
        address = "127.0.0.1";
        /*
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        socket.SendBufferSize = 0;
        socket.Connect(address, port);
        */

        mysocket.Connect(address, port);
        //mysocket.LaunceThread();
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("hello, this is client");
        mysocket.Send(buffer, buffer.Length);

        //socket.Send(buffer, buffer.Length, SocketFlags.None);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
