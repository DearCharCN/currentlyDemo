using System.Net.Sockets;

namespace DearChar.Net.Tcp
{
    internal static class TcpInternalUtls
    {
        internal static TcpClient[] ToClients(TcpChannel[] tcpChannels)
        {
            TcpClient[] result = new TcpClient[tcpChannels.Length];
            for (int i = 0; i < tcpChannels.Length; i++)
            {
                result[i] = ToClient(tcpChannels[i]);
            }
            return result;
        }

        internal static TcpChannel[] ToChannels(TcpClient[] tcpClients)
        {
            TcpChannel[] result = new TcpChannel[tcpClients.Length];
            for (int i = 0; i < tcpClients.Length; i++)
            {
                result[i] = ToChannel(tcpClients[i]);
            }
            return result;
        }

        internal static TcpClient ToClient(TcpChannel tcpChannel)
        {
            return tcpChannel.client;
        }

        internal static TcpChannel ToChannel(TcpClient tcpClient)
        {
            TcpChannel channel = new TcpChannel() { client = tcpClient };
            return channel;
        }
    }
}
