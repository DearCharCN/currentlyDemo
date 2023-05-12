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

        public TcpChannel Channel
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

        

        public TcpOneForOne(IPAddress iPAddress, int port) : base()
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
            lock (readResult)
            {
                if (readResult.Count > 0)
                {
                    var result = readResult.ToArray();
                    readResult.Clear();
                    return result;
                }
                return null;
            }
        }

        protected override void Awake()
        {
            TcpClient tcpClient = new TcpClient();
            serverChannel = new TcpChannel() { client = tcpClient };
            sender = new TcpSender();
            reader = new TcpReader();
            try
            {
                tcpClient.Connect(address, port);
            }
            catch (SocketException ex) 
            {
                Debug.LogException(ex);
                SetActive(false);
                return;
            }
        }

        protected override void OnEnable()
        {
            sender.SetActive(true);
            reader.SetActive(true);
        }

        protected override void Update()
        {
            DoReadTask();
        }

        protected override void OnDestroy()
        {
            sender.Destroy();
            reader.Destroy();

            var client = TcpInternalUtls.ToClient(serverChannel);
            if(client.Connected)
            {
                client.Close();
            }
        }

        ITcpReadTaskHandle readhandle;
        List<byte[]> readResult = new List<byte[]>();
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
                    readhandle = null;
                    lock (readResult)
                    {
                        foreach (var kv in r)
                        {
                            readResult.AddRange(kv.Value);
                        }
                    }
                }
            }
        }
    }
}