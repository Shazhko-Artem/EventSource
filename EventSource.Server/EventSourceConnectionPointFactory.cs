using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using EventSource.Common.Abstractions;
using EventSource.Common.Models;
using EventSource.Server.Options;
using Microsoft.Extensions.Options;

namespace EventSource.Server
{
    public class EventSourceConnectionPointFactory : IEventSourceConnectionPointFactory
    {
        private readonly EventSourceConnectionPointOptions options;

        public EventSourceConnectionPointFactory(IOptions<EventSourceConnectionPointOptions> options)
        {
            this.options = options.Value;
        }

        public EventSourceConnectionPoint Create()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress address = this.GetAvailableAddress(ipHostInfo.AddressList);
            IPEndPoint remoteEndPoint = new IPEndPoint(address, this.options.Port);

            Socket client = new Socket(address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            return new EventSourceConnectionPoint(client, remoteEndPoint);
        }

        private IPAddress GetAvailableAddress(IEnumerable<IPAddress> addresses)
        {
            return addresses.First(address => address.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}