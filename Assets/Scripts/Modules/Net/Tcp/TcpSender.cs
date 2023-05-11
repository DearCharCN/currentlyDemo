using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Threading;

namespace Net.Tcp
{
    internal class TcpSender : ThreadContainer
    {
        public TcpSender() : base(true)
        {
            tasks = new Queue<SendTask>();
        }

        Queue<SendTask> tasks;

        public void Send(TcpClient[] tcpClients, byte[] content)
        {
            tasks.Enqueue(new SendTask()
            {
                tcpClients = tcpClients,
                content = content
            });
        }

        class SendTask
        {
            internal TcpClient[] tcpClients;
            internal byte[] content;
        }

        protected override void Update()
        {
            if (tasks.Count > 0)
            {
                var t = tasks.Dequeue();
                DoSend(t.tcpClients, t.content);
            }
        }

        private void DoSend(TcpClient[] tcpClients, byte[] content)
        {
            for (int i = 0; i < tcpClients.Length; i++)
            {
                var s = tcpClients[i].GetStream();
                byte[] lenByte = BitConverter.GetBytes(content.Length);
                s.Write(NetConfigration.FLAGBytes, 0, NetConfigration.FLAGBytes.Length);//Flag
                s.Write(lenByte, 0, lenByte.Length);//³¤¶È
                s.Write(content, 0, content.Length);//ÄÚÈÝ
            }
        }
    }
}