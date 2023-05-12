using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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

        public void SendToAll(string msg,Encoding encoding = null)
        {
            TcpOneForMore.BroadPackage(msg, encoding);
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
            return tcpChannels != null && tcpChannels.Length > 0;
        }

        public bool HasAlreadlyDisconnected(out TcpChannel[] tcpChannels)
        {
            tcpChannels = TcpOneForMore.GetAlreadlyDisconnected();
            return tcpChannels != null && tcpChannels.Length > 0;
        }

        public void Dispose() 
        {
            TcpOneForMore.Destroy();
        }
    }
}

