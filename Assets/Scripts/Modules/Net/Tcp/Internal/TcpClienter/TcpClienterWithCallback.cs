using System;
using System.Net;
using DearChar;
using DearChar.Net.Tcp.Events;
using static UnityEditor.Progress;

namespace DearChar.Net.Tcp
{
    internal class TcpClienterWithCallback : TcpClienter
    {
        Action<EventData> onEvent;

        bool _thinkConnect = false;

        public TcpClienterWithCallback(IPAddress iPAddress, int port, bool Active = true) : base(iPAddress, port, Active)
        {
        }

        public void Addevent(Action<EventData> e)
        {
            onEvent += e;
        }

        public void RemoveEvent(Action<EventData> e)
        {
            onEvent -= e;
        }

        public new byte[][] GetPackage()
        {
            return null;
        }

        protected override void Start()
        {
            base.Start();
            _thinkConnect = true;
            try
            {
                EventData eventData = new EventData()
                {
                    channel = ServerChannel,
                    eventType = EventType.OnConnected,
                    data = null,
                };
                onEvent?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }

        protected override void Update()
        {
            base.Update();
            DoCb();
            CheckConnectType();
        }

        private void DoCb()
        {
            var packages = GetPackage();
            if (packages != null && packages.Length > 0)
            {
                packages.For((item) =>
                {
                    try
                    {
                        EventData eventData = new EventData()
                        {
                            channel = ServerChannel,
                            eventType = EventType.OnReadEnd,
                            data = item,
                        };
                        onEvent?.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            }
        }

        private void CheckConnectType()
        {
            if (_thinkConnect && !Connected)
            {
                _thinkConnect = false;
                try
                {
                    try
                    {
                        EventData eventData = new EventData()
                        {
                            channel = ServerChannel,
                            eventType = EventType.OnDisConnected,
                            data = null,
                        };
                        onEvent?.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

}