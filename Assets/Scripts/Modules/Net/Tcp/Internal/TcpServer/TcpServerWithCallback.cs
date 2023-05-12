using System;
using System.Collections.Generic;
using System.Net;
using DearChar.Net.Tcp.Events;

namespace DearChar.Net.Tcp
{
    internal class TcpServerWithCallback : TcpServer
    {
        Action<EventData> onEvent;

        public TcpServerWithCallback(IPAddress iPAddress, int port) : base(iPAddress, port)
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

        /// <summary>
        /// 这个方法不给用
        /// </summary>
        /// <returns></returns>
        public new Dictionary<TcpChannel, byte[][]> GetPackages()
        {
            return null;
        }

        /// <summary>
        /// 这个方法不给用
        /// </summary>
        /// <returns></returns>
        public new TcpChannel[] GetNewConnectChannel()
        {
            return null;
        }

        /// <summary>
        /// 这个方法不给用
        /// </summary>
        /// <returns></returns>
        public new TcpChannel[] GetAlreadlyDisconnected()
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
            DoReadEndEvent();
            DoOnConnectedEvent();
            DoDisConnectedEvent();
        }

        private void DoReadEndEvent()
        {
            if (onEvent != null)
            {
                Dictionary<TcpChannel, byte[][]> packages = base.GetPackages();

                foreach (var package in packages)
                {
                    var c = package.Key;
                    var ds = package.Value;
                    if (ds == null || ds.Length == 0)
                        continue;

                    ds.For((item, i) =>
                    {
                        try
                        {
                            EventData eventData = new EventData()
                            {
                                channel = c,
                                eventType = EventType.OnReadEnd,
                                data = ds[i],
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
        }

        private void DoOnConnectedEvent()
        {
            if (onEvent != null)
            {
                var channels = base.GetNewConnectChannel();
                channels.For((channel) =>
                {
                    try
                    {
                        EventData eventData = new EventData()
                        {
                            channel = channel,
                            eventType = EventType.OnConnected,
                            data = null,
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

        private void DoDisConnectedEvent()
        {
            if (onEvent != null)
            {
                var channels = base.GetAlreadlyDisconnected();
                channels.For((channel) =>
                {
                    try
                    {
                        EventData eventData = new EventData()
                        {
                            channel = channel,
                            eventType = EventType.OnDisConnected,
                            data = null,
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
    }
}