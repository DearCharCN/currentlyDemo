using DearChar.Net.Tcp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace DearChar.Threading
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

        public void StartServer()
        {
            TcpOneForOne.SetActive(true);
        }

        public void StopServer()
        {
            TcpOneForOne.SetActive(false);
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


