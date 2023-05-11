using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Net.Tcp
{
    internal class TcpServerWithCallback : TcpServer
    {
        Action<TcpChannel, byte[]> onReadEnd;

        public TcpServerWithCallback(IPAddress iPAddress, int port, bool Active = true) : base(iPAddress, port, Active)
        {
        }

        public void Addevent(Action<TcpChannel, byte[]> e)
        {
            onReadEnd += e;
        }

        public void RemoveEvent(Action<TcpChannel, byte[]> e)
        {
            onReadEnd -= e;
        }

        /// <summary>
        /// 这个方法不给用
        /// </summary>
        /// <returns></returns>
        public new Dictionary<TcpChannel, byte[][]> GetPackages()
        {
            return null;
        }

        protected override void Update()
        {
            base.Update();
            DoCb();
        }

        private void DoCb()
        {
            if (onReadEnd != null)
            {
                Dictionary<TcpChannel, byte[][]> packages = base.GetPackages();

                foreach (var package in packages)
                {
                    var c = package.Key;
                    var ds = package.Value;
                    if (ds == null || ds.Length == 0)
                        continue;

                    for (int i = 0; i < ds.Length; i++)
                    {
                        try
                        {
                            onReadEnd?.Invoke(c, ds[i]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
    }
}

