using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using UnityEditor.VersionControl;
using UnityEditor.PackageManager;
using System.IO;
using System.Linq.Expressions;
using UnityEngine.XR;

namespace Net
{
    public class TcpTest : MonoBehaviour
    {
        TcpServerListener_Old tcpServerListener;
        TcpClienter_Old tcpClienter;

        void Start()
        {
            TcpServerReader tcpServerReader = new TcpServerReader();
            Test3(tcpServerReader);
        }

        private void Test1(TcpServerReader tcpServerReader)
        {
            var data = GetPackage("123");
            List<byte[]> r;

            object tcpClient = new object();

            tcpServerReader.CheckPkg(tcpClient, data, data.Length,out r);
        }

        private void Test2(TcpServerReader tcpServerReader)
        {
            var data1 = GetPackage("123");
            var data2 = GetPackage("456");

            var data = Merage(data1, data2);

            List<byte[]> r;
            tcpServerReader.CheckPkg(new object(), data, data.Length, out r);
        }

        private void Test3(TcpServerReader tcpServerReader)
        {
            var data1 = GetPackage("123");
            var data2 = GetPackage("456");
            var data3 = GetPackage("dgdffhuyg");
            var data4 = GetPackage("gfhgjhg");
            var data5 = GetPackage("54545");
            var data6 = GetPackage("dgfytgdtf");

            var data = Merage(data1, data2, data3, data4, data5, data6);

            byte[][] d = SubCount(data,30);

            object tcpClient = new object();

            List<byte[]> r;
            foreach (var b in d)
            {
                tcpServerReader.CheckPkg(tcpClient, b, b.Length, out r);
                if(r != null)
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        Debug.Log(Encoding.UTF8.GetString(r[i]));
                    }
                }
            }
        }


        byte[][] SubCount(byte[] data, int count)
        {
            if (data.Length - 1 <= count)
                return null;
            List<byte[]> r = new List<byte[]>();
            r.AddRange(SubRamdom(data));
            for (int i = 1; i < count - 1; i++)
            {
                int splitIdx = FindLongst(r);
                byte[][] sqRes = SubRamdom(r[splitIdx]);
                r.RemoveAt(splitIdx);
                InsertRange(r, splitIdx, sqRes);
            }
            return r.ToArray();
        }

        int FindLongst(List<byte[]> list)
        {
            int maxLen = int.MinValue;
            int maxIdx = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i].Length > maxLen)
                {
                    maxLen = list[i].Length;
                    maxIdx = i;
                }
            }
            return maxIdx;
        }

        void InsertRange(List<byte[]> list,int pos, IEnumerable<byte[]> collection)
        {
            if(pos == list.Count)
            {
                list.AddRange(collection);
                return;
            }
            foreach(var d in collection)
            {
                list.Insert(pos,d);
                ++pos;
            }
        }

        byte[][] SubRamdom(byte[] data)
        {
            int s = UnityEngine.Random.Range(1, data.Length);
            return Sub(data, s);
        }

        byte[][] Sub(byte[] data,int split)
        {
            byte[] r1 = new byte[split];
            byte[] r2 = new byte[data.Length - split];
            
            Array.Copy(data, 0, r1, 0, split);
            Array.Copy(data, split, r2, 0, data.Length - split);
            byte[][] result = new byte[][] { r1, r2 };
            return result;
        }

        byte[] Merage(params byte[][] bytes)
        {
            int totalLen = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                totalLen += bytes[i].Length;
            }
            byte[] result = new byte[totalLen];

            int sumLen = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte[] data = bytes[i];
                Array.Copy(data, 0, result, sumLen, data.Length);
                sumLen += data.Length;
            }

            return result;
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

    internal class TcpListenerr
    {
        Thread thread;
        Action<TcpClient> cb;
        public TcpListenerr(int threadCount, Action<TcpClient> onPend)
        {
            cb = onPend;
            thread = new Thread(ListenMain);
            thread.Start();
        }

        public void Stop()
        {
            needStop = true;
        }

        public TcpClient[] GetPend()
        {
            List<TcpClient> prepareList = new List<TcpClient>();
            int len = notSend.Count;
            for (int i = 0; i < len; i++)
            {
                var c = notSend.Dequeue();
                if (c.Connected)
                {
                    prepareList.Add(c);
                }
            }
            return prepareList.ToArray();
        }

        bool needStop = false;
        private void ListenMain()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9000);
            tcpListener.Start();

            while (!needStop)
            {
                if (tcpListener.Pending())
                {
                    var client = tcpListener.AcceptTcpClient();

                    try
                    {
                        cb.Invoke(client);
                    }
                    catch
                    {
                        notSend.Enqueue(client);
                    }
                }
            }
            tcpListener.Stop();
        }

        Queue<TcpClient> notSend;
    }

    internal class TcpServerSender
    {
        const string FLAG = "[Dearchar]";
        static byte[] flagByte = Encoding.UTF8.GetBytes(FLAG);

        Thread thread;
        public TcpServerSender(int threadCount)
        {
            thread = new Thread(SendMain);
            thread.Start();
        }

        public void Stop()
        {
            needStop = true;
        }

        bool needStop = false;
        Queue<SendTask> tasks = new Queue<SendTask>();
        private void SendMain()
        {
            while (!needStop)
            {
                if (tasks.Count > 0)
                {
                    var t = tasks.Dequeue();
                    DoSend(t.tcpClients, t.content);
                }
            }
        }

        private void DoSend(TcpClient[] tcpClients, byte[] content)
        {
            for (int i = 0; i < tcpClients.Length; i++)
            {
                var s = tcpClients[i].GetStream();
                byte[] lenByte = BitConverter.GetBytes(content.Length);
                s.Write(flagByte, 0, flagByte.Length);//Flag
                s.Write(lenByte, 0, lenByte.Length);//长度
                s.Write(content, 0, content.Length);//内容
            }
        }

        class SendTask
        {
            internal TcpClient[] tcpClients;
            internal byte[] content;
        }

        public void Send(TcpClient[] tcpClients, byte[] content)
        {
            tasks.Enqueue(new SendTask()
            {
                tcpClients = tcpClients,
                content = content
            });
        }
    }

    internal class TcpServerReader
    {
        const string FLAG = "[Dearchar]";
        static byte[] flagByte = Encoding.UTF8.GetBytes(FLAG);

        Thread thread;
        public TcpServerReader()
        {
            thread = new Thread(ReadMain);
            thread.Start();
        }

        bool needStop = false;

        public void Stop()
        {
            needStop = true;
        }

        private void ReadMain()
        {
            while (!needStop)
            {
                for (int i = 0; i < connectedClient.Count; i++)
                {
                    if (!connectedClient[i].Connected)
                        continue;
                    DoRead(connectedClient[i]);
                }
            }
        }

        private void DoRead(TcpClient tcpClient)
        {
            var s = tcpClient.GetStream();
            byte[] bufer = new byte[1024];
            if (s.DataAvailable)
            {
                int len = s.Read(bufer, 0, bufer.Length);
                List<byte[]> c;
                if (!CheckPkg(tcpClient, bufer, len, out c))
                {
                    return;
                }

                for (int i = 0; i < c.Count; i++)
                {
                    ReadData readData = new ReadData()
                    {
                        tcpClient = tcpClient,
                        content = c[i]
                    };
                    readedCache.Enqueue(readData);
                }
            }
        }

        public bool CheckPkg(object tcpClient, byte[] bytes, int length, out List<byte[]> content)
        {
            content = null;
            byte[] merge = null;
            if (pkgCache.ContainsKey(tcpClient) && pkgCache[tcpClient] != null)
            {
                merge = new byte[pkgCache[tcpClient].Length + length];
                Array.Copy(pkgCache[tcpClient], 0, merge, 0, pkgCache[tcpClient].Length);
                Array.Copy(bytes, 0, merge, pkgCache[tcpClient].Length, length);
                pkgCache[tcpClient] = null;
            }
            else
            {
                merge = new byte[length];
                Array.Copy(bytes, 0, merge, 0, length);
            }
            content = SqlitPkg(tcpClient, merge);
            return content != null;
        }

        private List<byte[]> SqlitPkg(object tcpClient, byte[] data)
        {
            return SqlitPkg(tcpClient, data, 0, data.Length);
        }

        private List<byte[]> SqlitPkg(object tcpClient, byte[] data, int startIdx, int length)
        {
            int headerLen = (4 + flagByte.Length);
            if (length < headerLen)
            {
                SaveCachePkg(tcpClient, data, startIdx, length);
                return null;
            }

            if (!CheckFlagWithCloseTcp(tcpClient, data, startIdx))
            {
                return null;
            }

            int packageLen = GetInt32(data, startIdx + flagByte.Length);
            int relateLen = length - headerLen;
            if (relateLen < packageLen)
            {
                SaveCachePkg(tcpClient, data, startIdx, length);
                return null;
            }
            else
            {
                List<byte[]> result = new List<byte[]>();
                result.Add(Sub(data, startIdx + headerLen, packageLen));

                int relativeLen = length - headerLen - packageLen;
                if(relativeLen > 0)
                {
                    var subResult = SqlitPkg(tcpClient, data, startIdx + headerLen + packageLen, relativeLen);
                    if (subResult != null)
                    {
                        result.AddRange(subResult);
                    }
                }

                return result;
            }
        }

        private byte[] Sub(byte[] data, int start, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, start, result, 0, length);
            return result;
        }

        private int GetInt32(byte[] data, int startIdx)
        {
            return BitConverter.ToInt32(data, startIdx);
        }

        private bool CheckFlagWithCloseTcp(object tcpClient, byte[] data, int startIdx)
        {
            bool condition = CheckFlag(data, startIdx);
            if (!condition)
            {
                Debug.Log("[Tcp] Flag校验不正确,关闭链接");
                //tcpClient.Close();
            }
            return condition;
        }

        private bool CheckFlag(byte[] data, int startIdx)
        {
            if (startIdx + flagByte.Length >= data.Length)
                return false;

            return BytesEquals(data, startIdx, flagByte, 0, flagByte.Length);
        }

        private void SaveCachePkg(object tcpClient, byte[] data, int startIdx, int length)
        {
            if (startIdx + length > data.Length)
                return;

            byte[] cache = new byte[length];
            Array.Copy(data, startIdx, cache, 0, length);
            pkgCache[tcpClient] = cache;
        }

        Dictionary<object, byte[]> pkgCache = new Dictionary<object, byte[]>();

        private bool BytesEquals(byte[] a, int aStartIdx, byte[] b, int bStartIdx, int length)
        {
            int aIdx = aStartIdx;
            int bIdx = bStartIdx;
            int index = 0;

            while (index < length)
            {
                if (a.Length <= aIdx || b.Length <= bIdx)
                    return false;

                if (!a[aIdx].Equals(b[bIdx]))
                    return false;

                ++index;
                ++aIdx;
                ++bIdx;
            }
            return true;
        }

        List<TcpClient> connectedClient = new List<TcpClient>();

        internal class ReadData
        {
            public byte[] content;
            public TcpClient tcpClient;
        }
        Queue<ReadData> readedCache = new Queue<ReadData>();

        public ReadData[] Read()
        {
            int len = readedCache.Count;
            ReadData[] result = new ReadData[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = readedCache.Dequeue();
            }
            return result;
        }

        public void AddClent(TcpClient tcpClient)
        {
            if (tcpClient.Connected)
            {
                connectedClient.Add(tcpClient);
            }
        }

        public void RemoveClent(TcpClient tcpClient)
        {
            connectedClient.Remove(tcpClient);
        }
    }

    public enum DeviceNetType
    {
        Server,
        Client,
    }
}