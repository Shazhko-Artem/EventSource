using EventSource.Common.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace EventSource.Common
{
    public class ConnectionEndPointParser : IConnectionEndPointParser
    {
        private readonly ILogger<ConnectionEndPointParser> logger;

        public ConnectionEndPointParser(ILogger<ConnectionEndPointParser> logger)
        {
            this.logger = logger;
        }

        public IPEndPoint Parse(string connectionString)
        {
            this.logger.LogDebug($"[Parse] Start executing method.");
            IPAddress address;
            var uri = new Uri(connectionString);
            this.logger.LogDebug($"[Parse] Get connection string: '{connectionString}'");
            this.logger.LogDebug($"[Parse] HostNameType: '{uri.HostNameType}'");
            this.logger.LogDebug($"[Parse] Host: '{uri.Host}'");
            this.logger.LogDebug($"[Parse] Port: '{uri.Port}'");
            if (uri.HostNameType == UriHostNameType.IPv4)
            {
                address = IPAddress.Parse(uri.Host);
            }
            else
            {
                var hostInfo = Dns.GetHostEntry(uri.Host);
                address = this.GetAvailableAddress(hostInfo.AddressList);
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(address, uri.Port);
            this.logger.LogDebug($"[Parse] End executing method with value: '{remoteEndPoint.Address}:{remoteEndPoint.Port}'.");
            return remoteEndPoint;
        }

        private IPAddress GetAvailableAddress(IEnumerable<IPAddress> addresses)
        {
            return addresses.First(address => address.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}