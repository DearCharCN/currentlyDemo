using System;
using System.Collections.Generic;
using System.Net;

namespace DearChar.Net.Tcp
{
    public class TcpServer :IDisposable
    {
        TcpOneForMore TcpOneForMore;

        public TcpChannel[] Channels
        {
            get
            {
                return TcpOneForMore.Channels;
            }
        }


        public TcpServer(string ipArress, int port)
        {
            TcpOneForMore = new TcpOneForMore(IPAddress.Parse(ipArress), port);
        }

        public void StartServer()
        {
            TcpOneForMore.SetActive(true);
        }

        public void StopServer()
        {
            TcpOneForMore.SetActive(false);
        }

        public void Send(TcpChannel tcpChannel, byte[] data)
        {
            TcpOneForMore.SendPackage(tcpChannel, data);
        }

        public void SendToAll(byte[] data)
        {
            TcpOneForMore.BroadPackage(data);
        }

        public byte[][] Read(TcpChannel tcpChannel)
        {
            return TcpOneForMore.GetPackage(tcpChannel);
        }

        public Dictionary<TcpChannel, byte[][]> ReadAll()
        {
            return TcpOneForMore.GetPackages();
        }

        public bool HasNewChannelConnected(out TcpChannel[] tcpChannels)
        {
            tcpChannels = TcpOneForMore.GetNewConnectChannel();
            return tcpChannels != null;
        }

        public bool HasAlreadlyDisconnected(out TcpChannel[] tcpChannels)
        {
            tcpChannels = TcpOneForMore.GetAlreadlyDisconnected();
            return tcpChannels != null;
        }

        public void Dispose() 
        {
            TcpOneForMore.Destroy();
        }
    }
}

