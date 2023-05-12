using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DearChar.Threading;

namespace DearChar.Net.Tcp
{
    internal class TcpOneForOne : ThreadContainer
    {
        TcpReader reader;
        TcpSender sender;
        TcpChannel serverChannel;

        IPAddress address;
        int port;

        public TcpChannel ServerChannel
        {
            get
            {
                return serverChannel;
            }
        }

        public bool Connected
        {
            get
            {
                if(serverChannel == null)
                    return false;
                return TcpInternalUtls.ToClient(serverChannel).Connected;
            }
        }

        

        public TcpOneForOne(IPAddress iPAddress, int port, bool Active = true) : base(Active)
        {
            address = iPAddress;
            this.port = port;
        }

        public void SendPackage(string msg, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] data = encoding.GetBytes(msg);
            SendPackage(data);
        }

        public void SendPackage(byte[] bytes)
        {
            sender.Send(new TcpClient[] { serverChannel.client }, bytes);
        }

        public byte[][] GetPackage()
        {
            lock (readResultLock)
            {
                if (readResult.Count > 0)
                {
                    return readResult.ToArray();
                }
                return null;
            }
        }

        protected override void Start()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            serverChannel = new TcpChannel() {client = tcpClient };
            sender = new TcpSender();
            reader = new TcpReader();
        }

        protected override void Update()
        {
            DoReadTask();
        }

        ITcpReadTaskHandle readhandle;
        List<byte[]> readResult = new List<byte[]>();
        object readResultLock = new object();
        private void DoReadTask()
        {
            if (readhandle == null)
            {
                readhandle = reader.Read(new TcpClient[] { serverChannel.client });
            }
            else
            {
                Dictionary<TcpClient, byte[][]> r;
                if (reader.TryGetResult(readhandle, out r))
                {
                    foreach (var kv in r)
                    {
                        lock (readResultLock)
                        {
                            readResult.AddRange(kv.Value);
                        }
                    }
                }
            }
        }
    }
}