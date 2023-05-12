using System.Collections.Generic;
using DearChar.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DearChar.Net.Tcp
{
    internal class TcpServer : ThreadContainer
    {
        TcpListener listener;
        TcpSender sender;
        TcpReader reader;

        List<TcpChannel> connectedClient;

        public TcpChannel[] Channels
        {
            get
            {
                return connectedClient.ToArray();
            }
        }

        public TcpServer(IPAddress iPAddress, int port, bool Active = true) : base(Active)
        {
            listener = new TcpListener(iPAddress, port, Active);
            sender = new TcpSender();
            reader = new TcpReader();
            connectedClient = new List<TcpChannel>();
        }



        public void BroadPackage(string msg, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] data = encoding.GetBytes(msg);

            var channels = Channels;
            TcpClient[] tcpClients = TcpInternalUtls.ToClients(channels);
            SendPackage(tcpClients, data);
        }

        public void SendPackage(TcpChannel channel, string msg, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] data = encoding.GetBytes(msg);
            SendPackage(channel, data);
        }

        public void SendPackage(TcpChannel channel, byte[] msg)
        {
            TcpClient tcpClients = channel.client;
            SendPackage(new TcpClient[] { tcpClients }, msg);
        }

        public byte[][] GetPackage(TcpChannel channel)
        {
            lock (readResultLock)
            {
                var client = channel.client;
                if (!readResult.ContainsKey(client))
                {
                    return null;
                }

                var result = readResult[client].ToArray();
                readResult.Remove(client);
                return result;
            }
        }

        public Dictionary<TcpChannel, byte[][]> GetPackages()
        {
            Dictionary<TcpChannel, byte[][]> result = new Dictionary<TcpChannel, byte[][]>();
            lock (readResultLock)
            {
                foreach (var kv in readResult)
                {
                    if (kv.Value == null)
                        continue;

                    TcpChannel channel = new TcpChannel()
                    {
                        client = kv.Key,
                    };
                    result[channel] = kv.Value.ToArray();
                }
                readResult.Clear();
                return result;
            }
        }

        public void CloseChannel(TcpChannel channel)
        {
            if (channel.client.Connected)
            {
                channel.client.Close();
            }
        }

        private void SendPackage(TcpClient[] tcpClients, byte[] bytes)
        {
            sender.Send(tcpClients, bytes);
        }

        protected override void Update()
        {
            GetConnectClients();
            DoReadTask();
            ClearDisConnectedClients();
        }

        private void GetConnectClients()
        {
            var connted = listener.GetConnectedClients();
            if (connted != null && connted.Length > 0)
            {
                for (int i = 0; i < connted.Length; i++)
                {
                    var client = connted[i];
                    if (!client.Connected)
                    {
                        continue;
                    }
                    connectedClient.Add(new TcpChannel()
                    {
                        client = client
                    });
                }
            }
        }

        ITcpReadTaskHandle readhandle;
        Dictionary<TcpClient, List<byte[]>> readResult = new Dictionary<TcpClient, List<byte[]>>();
        object readResultLock = new object();
        private void DoReadTask()
        {
            if (readhandle == null)
            {
                readhandle = reader.Read(TcpInternalUtls.ToClients(Channels));
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
                            if (!readResult.ContainsKey(kv.Key))
                            {
                                readResult[kv.Key] = new List<byte[]>();
                            }
                            readResult[kv.Key].AddRange(kv.Value);
                        }
                    }
                }
            }
        }

        private void ClearDisConnectedClients()
        {
            for (int i = 0; i < connectedClient.Count; i++)
            {
                TcpClient tcpClient = connectedClient[i].client;
                if (!tcpClient.Connected)
                {
                    connectedClient.RemoveAt(i);
                    --i;
                }
            }
        }
    }

    internal class TcpChannel
    {
        internal TcpClient client;
    }
}

