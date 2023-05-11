using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DearChar.Threading;

namespace DearChar.Net.Tcp
{
    internal partial class TcpReader: ThreadContainer
    {
        internal TcpReader() : base(true)
        {
        }
        public ITcpReadTaskHandle Read(TcpClient[] tcpClients)
        {
            uint taskId = taskIdAllocation.GetTaskId();
            readTasks.Enqueue(new ReadTask()
            {
                taskId = taskId,
                tcpClients = tcpClients,
            });
            return new TcpReadTaskHandle() { taskId = taskId };
        }

        public bool TryGetResult(ITcpReadTaskHandle taskHandle,out Dictionary<TcpClient, byte[][]> result)
        {
            result = null;
            TcpReadTaskHandle handle = taskHandle as TcpReadTaskHandle;

            if (handle.isDispose)
                throw new Exception("This handle is not vaid");

            if (!taskResults.ContainsKey(handle.taskId))
            {
                return false;
            }

            result = taskResults[handle.taskId].datas;

            taskResults.Remove(handle.taskId);
            taskIdAllocation.ReleseTaskId(handle.taskId);
            handle.isDispose = true;
            return true;
        }

        Queue<ReadTask> readTasks = new Queue<ReadTask>();

        Dictionary<uint, TaskResult> taskResults = new Dictionary<uint, TaskResult>();

        class ReadTask
        {
            internal uint taskId;
            internal TcpClient[] tcpClients;
        }

        class TaskResult
        {
            internal uint taskId;
            internal Dictionary<TcpClient, byte[][]> datas;
        }

        protected override void Update()
        {
            int count = readTasks.Count;
            for (int i = 0; i < count; i++)
            {
                var task = readTasks.Dequeue();
                DoTask(task);
            }
        }

        private void DoTask(ReadTask readTask)
        {
            TaskResult taskResult = new TaskResult();
            taskResult.taskId = readTask.taskId;
            taskResult.datas = new Dictionary<TcpClient, byte[][]>();

            for (int i = 0; i < readTask.tcpClients.Length; i++)
            {
                var client = readTask.tcpClients[i];
                if(client.Connected)
                {
                    byte[][] bytes = DoRead(client);

                    if(bytes != null)
                    {
                        taskResult.datas[client] = bytes;
                    }
                }
            }

            taskResults[readTask.taskId] = taskResult;
        }

        private byte[][] DoRead(TcpClient tcpClient)
        {
            var s = tcpClient.GetStream();
            byte[] bufer = new byte[1024];
            if (s.DataAvailable)
            {
                int len = s.Read(bufer, 0, bufer.Length);
                List<byte[]> c;
                if (!packageCutter.GetPackages(tcpClient, bufer, len, out c))
                {
                    return null;
                }
                return c.ToArray();
            }
            return null;
        }
    }

    internal partial class TcpReader
    {
        TaskIdAllocation taskIdAllocation = new TaskIdAllocation();
        internal class TaskIdAllocation
        {
            uint currently = uint.MinValue;
            Dictionary<uint, uint> usedTaskId = new Dictionary<uint, uint>();
            object lockObj = new object();
            public uint GetTaskId()
            {
                lock(lockObj)
                {
                    uint id = currently;
                    while (usedTaskId.ContainsKey(id))
                    {
                        AddCurrently();
                        id = currently;
                    }
                    usedTaskId[id] = id;
                    AddCurrently();

                    return id;
                }
            }

            private void AddCurrently()
            {
                ++currently;
                if (currently == int.MaxValue)
                {
                    currently = uint.MinValue;
                }
            }

            public void ReleseTaskId(uint taskId)
            {
                lock (lockObj)
                {
                    usedTaskId.Remove(taskId);
                }
            }
        }
    }

    internal interface ITcpReadTaskHandle
    {
    }
    internal partial class TcpReader
    {
        internal class TcpReadTaskHandle : ITcpReadTaskHandle
        {
            internal uint taskId;

            internal bool isDispose = false;
        }
    }
    /// <summary>
    /// PackageCutter
    /// </summary>
    internal partial class TcpReader
    {
        PackageCutter packageCutter = new PackageCutter();
        private class PackageCutter
        {
            public bool GetPackages(object tcpClient, byte[] bytes, int length, out List<byte[]> packages)
            {
                packages = null;
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
                packages = CutPackages(tcpClient, merge);
                return packages != null;
            }

            private List<byte[]> CutPackages(object tcpClient, byte[] data)
            {
                return CutPackages(tcpClient, data, 0, data.Length);
            }

            private List<byte[]> CutPackages(object tcpClient, byte[] data, int startIdx, int length)
            {
                int headerLen = (4 + NetConfigration.FLAGBytes.Length);
                if (length < headerLen)
                {
                    SaveIncompletePackage(tcpClient, data, startIdx, length);
                    return null;
                }

                if (!EqaulFlagWithCloseTcp(tcpClient, data, startIdx))
                {
                    return null;
                }

                int packageLen = BitUtls.GetInt32(data, startIdx + NetConfigration.FLAGBytes.Length);
                int relateLen = length - headerLen;
                if (relateLen < packageLen)
                {
                    SaveIncompletePackage(tcpClient, data, startIdx, length);
                    return null;
                }
                else
                {
                    List<byte[]> result = new List<byte[]>();
                    result.Add(BitUtls.SubBytes(data, startIdx + headerLen, packageLen));

                    int relativeLen = length - headerLen - packageLen;
                    if (relativeLen > 0)
                    {
                        var subResult = CutPackages(tcpClient, data, startIdx + headerLen + packageLen, relativeLen);
                        if (subResult != null)
                        {
                            result.AddRange(subResult);
                        }
                    }

                    return result;
                }
            }

            private bool EqaulFlagWithCloseTcp(object tcpClient, byte[] data, int startIdx)
            {
                bool condition = EqaulFlag(data, startIdx);
                if (!condition)
                {
                    Debug.Log("[Tcp] Flag校验不正确,关闭链接");
                    (tcpClient as TcpClient).Close();
                }
                return condition;
            }

            private bool EqaulFlag(byte[] data, int startIdx)
            {
                if (startIdx + NetConfigration.FLAGBytes.Length >= data.Length)
                    return false;

                return BitUtls.BytesEquals(data, startIdx, NetConfigration.FLAGBytes, 0, NetConfigration.FLAGBytes.Length);
            }

            private void SaveIncompletePackage(object tcpClient, byte[] data, int startIdx, int length)
            {
                if (startIdx + length > data.Length)
                    return;

                byte[] cache = new byte[length];
                Array.Copy(data, startIdx, cache, 0, length);
                pkgCache[tcpClient] = cache;
            }

            Dictionary<object, byte[]> pkgCache = new Dictionary<object, byte[]>();
        }
    }
}