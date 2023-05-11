using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


namespace Net.Tcp
{
    public static class TcpUtls
    {

    }

    internal static class TcpInternalUtls
    {
        internal static TcpClient[] ToClients(TcpChannel[] tcpChannels)
        {
            TcpClient[] result = new TcpClient[tcpChannels.Length];
            for (int i = 0; i < tcpChannels.Length; i++)
            {
                TcpChannel channel = tcpChannels[i];
                result[i] = channel.client;
            }
            return result;
        }
    }
}
