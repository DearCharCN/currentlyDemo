using UnityEngine;
using DearChar.Net.Tcp;
using UnityEngine.UI;
using System.Text;

public class TcpTesterer : MonoBehaviour
{
    public DeviceNetType DeviceNetType;

    TcpServer server;
    TcpClienter client;

    void Start()
    {
        if (DeviceNetType == DeviceNetType.Client)
        {
            client = new TcpClienter("127.0.0.1", 9000);
            client.StartClienter();
        }
        else
        {
            server = new TcpServer("127.0.0.1", 9000);
            server.StartServer();
        }
    }

    private void OnDestroy()
    {
        if (DeviceNetType == DeviceNetType.Client)
        {
            client.Dispose();
        }
        else
        {
            server.Dispose();
        }
    }

    public void OnClick()
    {
        if (DeviceNetType == DeviceNetType.Client)
        {
            client.Send("123");
        }
        else
        {
            server.SendToAll("I am server");
        }
    }

    bool _isConnected = false;

    void Update()
    {
        if (DeviceNetType == DeviceNetType.Client)
        {
            if (!_isConnected && client.Connected)
            {
                _isConnected = true;
                Debug.Log("已连接服务器");
            }
            if (client.Connected)
            {
                var connect = client.Read();
                if (connect != null && connect.Length > 0)
                {
                    connect.For((item) =>
                    {
                        Debug.Log(Encoding.UTF8.GetString(item));
                    });
                }
            }
        }
        else
        {
            TcpChannel[] channels;
            if (server.HasNewChannelConnected(out channels))
            {
                Debug.Log($"{channels.Length}台客户端已连接");
            }
            if (server.HasAlreadlyDisconnected(out channels))
            {
                Debug.Log($"{channels.Length}台客户端已断开连接");
            }

            if (server.Channels != null && server.Channels.Length > 0)
            {
                var content = server.ReadAll();
                if (content != null && content.Count > 0)
                {
                    foreach (var kv in content)
                    {
                        kv.Value.For((item) =>
                        {
                            Debug.Log(Encoding.UTF8.GetString(item));
                        });
                    }
                }
            }
        }
    }
}
