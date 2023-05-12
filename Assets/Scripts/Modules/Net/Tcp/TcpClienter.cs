using System;
using System.Net;
using System.Text;

namespace DearChar.Net.Tcp
{
    public class TcpClienter : IDisposable
    {
        TcpOneForOne TcpOneForOne;

        public TcpChannel Server
        {
            get
            {
                return TcpOneForOne.Channel;
            }
        }

        public bool Connected
        {
            get
            {
                return TcpOneForOne.Connected;
            }
        }


        public TcpClienter(string ipArress, int port)
        {
            TcpOneForOne = new TcpOneForOne(IPAddress.Parse(ipArress), port);
        }

        public void StartClienter()
        {
            TcpOneForOne.SetActive(true);
        }

        public void StopClienter()
        {
            TcpOneForOne.SetActive(false);
        }

        public void Send(string msg,Encoding encoding = null)
        {
            TcpOneForOne.SendPackage(msg, encoding);
        }

        public void Send(byte[] data)
        {
            TcpOneForOne.SendPackage(data);
        }

        public byte[][] Read()
        {
            return TcpOneForOne.GetPackage();
        }

        public void Dispose()
        {
            TcpOneForOne.Destroy();
        }
    }
}


