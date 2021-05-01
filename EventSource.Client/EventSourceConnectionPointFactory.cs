using System;
using System.Collections.Generic;
using System.Linq;
using EventSource.Common.Abstractions;
using EventSource.Common.Models;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace EventSource.Client
{
    public class EventSourceConnectionPointFactory : IEventSourceConnectionPointFactory
    {
        private readonly string connectionString;
        private readonly ILogger<EventSourceConnectionPointFactory> logger;

        public EventSourceConnectionPointFactory(string connectionString, ILogger<EventSourceConnectionPointFactory> logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public EventSourceConnectionPoint Create()
        {
            IPAddress address;
            var uri = new Uri(this.connectionString);
            this.logger.LogInformation($"[Create] connectionString: '{this.connectionString}'");
            this.logger.LogInformation($"[Create] uri.HostNameType: '{uri.HostNameType}'");
            this.logger.LogInformation($"[Create] uri.Host: '{uri.Host}'");
            this.logger.LogInformation($"[Create] uri.AbsolutePath: '{uri.AbsolutePath}'");
            this.logger.LogInformation($"[Create] uri.AbsoluteUri: '{uri.AbsoluteUri}'");
            this.logger.LogInformation($"[Create] uri.OriginalString: '{uri.OriginalString}'");
            this.logger.LogInformation($"[Create] uri.LocalPath: '{uri.LocalPath}'");
            this.logger.LogInformation($"[Create] uri.IdnHost: '{uri.IdnHost}'");
            this.logger.LogInformation($"[Create] uri.Fragment: '{uri.Fragment}'");
            this.logger.LogInformation($"[Create] uri.IsUnc: '{uri.IsUnc}'");
            if (uri.HostNameType == UriHostNameType.IPv4)
            {
                address = IPAddress.Parse(uri.Host);
            }
            else
            {
                var hostInfo = Dns.GetHostEntry(uri.Host);
                this.logger.LogInformation($"[Create] hostInfo.AddressList: '{hostInfo.AddressList}'");
                address = this.GetAvailableAddress(hostInfo.AddressList);
            }

            this.logger.LogInformation($"[Create] uri.Port: '{uri.Port}'");
            IPEndPoint remoteEndPoint = new IPEndPoint(address, uri.Port);
            Socket client = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return new EventSourceConnectionPoint(client, remoteEndPoint);
        }

        private IPAddress GetAvailableAddress(IEnumerable<IPAddress> addresses)
        {
            return addresses.First(address => address.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}