using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DearChar.Threading;

namespace DearChar.Net.Tcp
{
    internal class TcpSender : ThreadContainer
    {
        Queue<SendTask> tasks = new Queue<SendTask>();

        public void Send(TcpClient[] tcpClients, byte[] content)
        {
            lock(tasks)
            {
                tasks.Enqueue(new SendTask()
                {
                    tcpClients = tcpClients,
                    content = content
                });
            }
        }

        class SendTask
        {
            internal TcpClient[] tcpClients;
            internal byte[] content;
        }

        protected override void Update()
        {
            lock(tasks)
            {
                if (tasks.Count > 0)
                {
                    var t = tasks.Dequeue();
                    DoSend(t.tcpClients, t.content);
                }
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