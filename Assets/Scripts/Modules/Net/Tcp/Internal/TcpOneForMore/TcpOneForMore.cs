using System.Collections.Generic;
using DearChar.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Net.Http;

namespace DearChar.Net.Tcp
{
    internal class TcpOneForMore : ThreadContainer
    {
        TcpListener listener;
        TcpSender sender;
        TcpReader reader;

        IPAddress iPAddress;
        int port;

        List<TcpChannel> connectedClient = new List<TcpChannel>();

        public TcpChannel[] Channels
        {
            get
            {
                lock(connectedClient)
                {
                    return connectedClient.ToArray();
                }
            }
        }

        public TcpOneForMore(IPAddress iPAddress, int port) : base()
        {
            this.iPAddress = iPAddress;
            this.port = port;
        }



        public void BroadPackage(string msg, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] data = encoding.GetBytes(msg);

            BroadPackage(data);
        }

        public void BroadPackage(byte[] data)
        {
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
            TcpClient tcpClients = TcpInternalUtls.ToClient(channel);
            SendPackage(new TcpClient[] { tcpClients }, msg);
        }

        public byte[][] GetPackage(TcpChannel channel)
        {
            lock (readResult)
            {
                var client = TcpInternalUtls.ToClient(channel);
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
            lock (readResult)
            {
                foreach (var kv in readResult)
                {
                    if (kv.Value == null)
                        continue;

                    
                    TcpChannel channel = TcpInternalUtls.ToChannel(kv.Key);
                    result[channel] = kv.Value.ToArray();
                }
                readResult.Clear();
                return result;
            }
        }

        public void CloseChannel(TcpChannel channel)
        {
            var client = TcpInternalUtls.ToClient(channel);
            if (client.Connected)
            {
                client.Close();
            }
        }

        public TcpChannel[] GetNewConnectChannel()
        {
            lock (newConnected)
            {
                var result = newConnected.ToArray();
                newConnected.Clear();
                return result;
            }
        }

        public TcpChannel[] GetAlreadlyDisconnected()
        {
            lock (disconnectList)
            {
                var result = disconnectList.ToArray();
                disconnectList.Clear();
                return result;
            }
        }

        private void SendPackage(TcpClient[] tcpClients, byte[] bytes)
        {
            sender.Send(tcpClients, bytes);
        }

        protected override void Awake()
        {
            listener = new TcpListener(iPAddress, port);
            sender = new TcpSender();
            reader = new TcpReader();
        }

        protected override void OnEnable()
        {
            listener.SetActive(true);
            sender.SetActive(true);
            reader.SetActive(true);
        }

        protected override void OnDisable()
        {
            listener.SetActive(false);
            sender.SetActive(false);
            reader.SetActive(false);
        }

        protected override void Update()
        {
            GetConnectClients();
            DoReadTask();
            ClearDisConnectedClients();
        }

        protected override void OnDestroy()
        {
            listener.Destroy();
            sender.Destroy();
            reader.Destroy();

            connectedClient.For((item) =>
            {
                var client = TcpInternalUtls.ToClient(item);
                if(client.Connected)
                {
                    client.Close();
                    client.Dispose();

                }
            });
            connectedClient.Clear();
        }

        List<TcpChannel> newConnected = new List<TcpChannel>();

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

                    var channel = TcpInternalUtls.ToChannel(client);
                    connectedClient.Add(channel);

                    lock(newConnected)
                    {
                        newConnected.Add(channel);
                    }
                }
            }
        }

        ITcpReadTaskHandle readhandle;
        Dictionary<TcpClient, List<byte[]>> readResult = new Dictionary<TcpClient, List<byte[]>>();
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
                    readhandle = null;
                    foreach (var kv in r)
                    {
                        lock (readResult)
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

        List<TcpChannel> disconnectList = new List<TcpChannel>();

        private void ClearDisConnectedClients()
        {
            for (int i = 0; i < connectedClient.Count; i++)
            {
                TcpClient tcpClient = TcpInternalUtls.ToClient(connectedClient[i]);
                if (!tcpClient.Connected)
                {
                    connectedClient.RemoveAt(i);
                    --i;
                    lock(disconnectList)
                    {
                        disconnectList.Add(TcpInternalUtls.ToChannel(tcpClient));
                    }
                }
            }
        }
    }

    public class TcpChannel
    {
        public TcpClient client;
    }
}