using System.Net;
using System.Net.Sockets;
using Threading;

namespace Net.Tcp
{
    internal class TcpClienter : ThreadContainer
    {
        TcpReader reader;
        TcpSender sender;
        TcpChannel serverChannel;

        IPAddress address;
        int port;

        public TcpClienter(IPAddress iPAddress, int port, bool Active = true) : base(Active)
        {
            address = iPAddress;
            this.port = port;
        }

        protected override void Start()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            serverChannel = new TcpChannel() {client = tcpClient };
            sender = new TcpSender();
            reader = new TcpReader();
        }

        protected override void Update()
        {
            
        }
    }
}