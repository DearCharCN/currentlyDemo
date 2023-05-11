using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Threading;

namespace Net.Tcp
{
    internal class TcpConnecter : ThreadContainer
    {
        TcpListener tcpListener;
        IPAddress address;
        int port;

        Queue<TcpClient> tcpClients;

        public TcpConnecter(IPAddress iPAddress, int port, bool Active = true) : base(Active)
        {
            this.port = port;
            this.address = iPAddress;
        }

        public TcpClient[] GetConnectedClients()
        {
            List<TcpClient> prepareList = new List<TcpClient>();
            int len = tcpClients.Count;
            for (int i = 0; i < len; i++)
            {
                var c = tcpClients.Dequeue();
                if (c.Connected)
                {
                    prepareList.Add(c);
                }
            }
            return prepareList.ToArray();
        }

        protected override void Awake()
        {
            tcpClients = new Queue<TcpClient>();
            tcpListener = new TcpListener(address, port);
            tcpListener.Start();
        }

        protected override void Update()
        {
            if (tcpListener.Pending())
            {
                var client = tcpListener.AcceptTcpClient();
                tcpClients.Enqueue(client);
            }
        }

        protected override void OnDestroy()
        {
            tcpListener.Stop();
        }
    }
}