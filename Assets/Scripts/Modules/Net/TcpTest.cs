using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Reflection;

namespace Net.Tcp
{
    public class PkgCutterTester : MonoBehaviour
    {

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
        }
    }
}