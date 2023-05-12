using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using DearChar.Threading;

namespace DearChar.Net.Tcp
{
    internal class TcpListener : ThreadContainer
    {
        System.Net.Sockets.TcpListener tcpListener;
        IPAddress address;
        int port;

        Queue<TcpClient> connectedCaches;

        public TcpListener(IPAddress iPAddress, int port) : base()
        {
            this.port = port;
            this.address = iPAddress;
        }

        public TcpClient[] GetConnectedClients()
        {
            int len = connectedCaches.Count;
            List<TcpClient> prepareList = new List<TcpClient>();
            for (int i = 0; i < len; i++)
            {
                var c = connectedCaches.Dequeue();
                if (c.Connected)
                {
                    prepareList.Add(c);
                }
            }
            return prepareList.ToArray();
        }

        protected override void Awake()
        {
            connectedCaches = new Queue<TcpClient>();
            tcpListener = new System.Net.Sockets.TcpListener(address, port);
            tcpListener.Start();
        }

        protected override void Update()
        {
            if (tcpListener.Pending())
            {
                var client = tcpListener.AcceptTcpClient();
                connectedCaches.Enqueue(client);
            }
        }

        protected override void OnDestroy()
        {
            tcpListener.Stop();
        }
    }
}