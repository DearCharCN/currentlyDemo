using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using DearChar.Net;
using DearChar.Net.Tcp;
using System.Net;
using System.Net.Http;

public class TCPTest : MonoBehaviour
{
    public DeviceNetType DeviceType;

    TcpClient c;

    TcpListener tcpListener;

    // Start is called before the first frame update
    void Start()
    {
        if (DeviceType == DeviceNetType.Client)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 9000);
            c = tcpClient;
            Debug.Log("连接");
        }
        else
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
            tcpListener.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DeviceType == DeviceNetType.Server)
        {
            if (tcpListener.Pending())
            {
                c = tcpListener.AcceptTcpClient();
                Debug.Log("连接");
            }
        }

        if (c != null)
        {
            Debug.Log($"connect = {c.Connected}");
        }

    }

    public void OnClick()
    {
        c.Client.Disconnect(false);
        c.Close();
        c.Dispose();
        c = null;
    }
}
