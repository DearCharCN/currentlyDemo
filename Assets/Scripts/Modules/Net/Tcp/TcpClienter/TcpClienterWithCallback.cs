using System;
using System.Net;
using DearChar;

namespace DearChar.Net.Tcp
{
    internal class TcpClienterWithCallback : TcpClienter
    {
        Action<byte[]> onReadEnd;

        public TcpClienterWithCallback(IPAddress iPAddress, int port, bool Active = true) : base(iPAddress, port, Active)
        {
        }

        public void Addevent(Action<byte[]> e)
        {
            onReadEnd += e;
        }

        public void RemoveEvent(Action<byte[]> e)
        {
            onReadEnd -= e;
        }

        public new byte[][] GetPackage()
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
            var packages = GetPackage();
            if (packages != null && packages.Length > 0)
            {
                for (int i = 0; i < packages.Length; i++)
                {
                    try
                    {
                        onReadEnd?.Invoke(packages[i]);
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