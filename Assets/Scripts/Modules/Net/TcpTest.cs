using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using System.Reflection;

namespace Net.Tcp
{
    public class TcpTest : MonoBehaviour
    {
        TcpServerListener_Old tcpServerListener;
        TcpClienter_Old tcpClienter;

        MethodInfo GetPackagesMethod = typeof(TcpReader).GetMethod("GetPackages");

        void Start()
        {
            TcpReader tcpServerReader = new TcpReader();
            Test3(tcpServerReader);
        }

        private void Test1(TcpReader tcpServerReader)
        {
            var data = GetPackage("123");
            List<byte[]> r = null;

            object tcpClient = new object();


            List<object> parameters = new List<object>();
            parameters.Add(tcpClient);
            parameters.Add(data);
            parameters.Add(data.Length);
            parameters.Add(r);

            GetPackagesMethod.Invoke(tcpServerReader, parameters.ToArray());
        }

        private void Test2(TcpReader tcpServerReader)
        {
            var data1 = GetPackage("123");
            var data2 = GetPackage("456");

            var data = ArrayUtls.Merage(data1, data2);

            List<byte[]> r = null;

            List<object> parameters = new List<object>();
            parameters.Add(new object());
            parameters.Add(data);
            parameters.Add(data.Length);
            parameters.Add(r);

            GetPackagesMethod.Invoke(tcpServerReader, parameters.ToArray());
        }

        private void Test3(TcpReader tcpServerReader)
        {
            var data1 = GetPackage("123");
            var data2 = GetPackage("456");
            var data3 = GetPackage("dgdffhuyg");
            var data4 = GetPackage("gfhgjhg");
            var data5 = GetPackage("54545");
            var data6 = GetPackage("dgfytgdtf");

            var data = ArrayUtls.Merage(data1, data2, data3, data4, data5, data6);

            byte[][] d = ArrayUtls.CutOffByCount(data, 30);

            object tcpClient = new object();

            List<byte[]> r = null;
            foreach (var b in d)
            {
                List<object> parameters = new List<object>();
                parameters.Add(tcpClient);
                parameters.Add(b);
                parameters.Add(b.Length);
                parameters.Add(r);

                GetPackagesMethod.Invoke(tcpServerReader, parameters.ToArray());
                if (r != null)
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        Debug.Log(Encoding.UTF8.GetString(r[i]));
                    }
                }
            }
        }

        byte[] GetPackage(string message)
        {
            return GetTcpPackae(Encoding.UTF8.GetBytes(message));
        }

        byte[] GetTcpPackae(byte[] content)
        {
            byte[] flag = Encoding.UTF8.GetBytes("[Dearchar]");
            byte[] len = BitConverter.GetBytes(content.Length);
            byte[] result = new byte[flag.Length + len.Length + content.Length];
            Array.Copy(flag, 0, result, 0, flag.Length);
            Array.Copy(len, 0, result, flag.Length, len.Length);
            Array.Copy(content, 0, result, flag.Length + len.Length, content.Length);
            return result;
        }

        private void OnDestroy()
        {
            //tcpServerListener.Stop();
            //tcpClienter.Stop();
        }
    }

    internal class TcpServerListener_Old
    {
        Thread thread;

        public TcpServerListener_Old()
        {
            thread = new Thread(ThreadMain);
            thread.Start();
        }

        public void Stop()
        {
            isNeedStop = true;
        }

        bool isNeedStop = false;
        private void ThreadMain()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
            tcpListener.Start();
            NetworkStream stream = null;
            TcpClient client = null;

            if (tcpListener.Pending())
            {
                client = tcpListener.AcceptTcpClient();
                Debug.Log("[Server] Connected");
                stream = client.GetStream();
            }
            tcpListener.Stop();

            while (client.Connected && !isNeedStop)
            {
                string str = $"我是服务端： {DateTime.Now}";
                byte[] datas = Encoding.UTF8.GetBytes(str);
                stream.Write(datas, 0, datas.Length);
                Debug.Log($"[Server] send msg {str}");

                if (stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int len = stream.Read(buffer, 0, buffer.Length);

                    string message = Encoding.UTF8.GetString(buffer, 0, len);
                    UnityEngine.Debug.Log($"[Server] Recive msg {message}");
                }
            }

            if (client.Connected)
            {
                client.Dispose();
            }
            Debug.Log($"[Server] Stop");
        }
    }

    internal class TcpClienter_Old
    {
        Thread thread;

        public TcpClienter_Old()
        {
            thread = new Thread(ThreadMain);
            thread.Start();
        }

        public void Stop()
        {
            isNeedStop = true;
        }

        bool isNeedStop = false;

        public void ThreadMain()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 9000);
            Debug.Log($"[Client] Connected");
            NetworkStream netStream = tcpClient.GetStream();

            while (tcpClient.Connected && !isNeedStop)
            {
                string str = $"当前客户端时间： {DateTime.Now}";
                byte[] datas = Encoding.UTF8.GetBytes(str);
                netStream.Write(datas, 0, datas.Length);
                Debug.Log($"[Client] send msg {str}");

                if (netStream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int len = netStream.Read(buffer, 0, buffer.Length);

                    string message = Encoding.UTF8.GetString(buffer, 0, len);
                    UnityEngine.Debug.Log($"[Client] Recive msg {message}");
                }

            }




            netStream.Close();
            tcpClient.Close();
            Debug.Log($"[Client] Stop");
        }
    }
}