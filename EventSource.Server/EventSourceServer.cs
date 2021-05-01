using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using EventSource.Common.Convertors;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;
using EventSource.Server.Abstractions;
using EventSource.Server.Models;
using Microsoft.Extensions.Logging;

namespace EventSource.Server
{
    public class EventSourceServer : IEventSourceServer
    {
        private readonly List<Socket> clients = new List<Socket>();
        private readonly IEventSourceConnection connection;
        private readonly ILogger<EventSourceServer> logger;

        public EventSourceServer(
            IEventSourceConnection connection,
            ILogger<EventSourceServer> logger)
        {
            this.connection = connection;
            this.logger = logger;
            this.connection.OnClientConnected += HandleConnectedClient;
            this.connection.OnClientDisconnected += HandleDisconnectedClient;
            this.connection.OnReceivedData += HandleReceivedData;
        }

        public int ClientsCount => this.clients.Count;

        public void Send(EventMessage data)
        {
            this.logger.LogDebug($"Send event '{data.Name}'");
            this.Send(data, this.clients);
        }

        public void Dispose()
        {
            this.logger.LogDebug("Dispose event source server");
            this.connection.OnClientConnected -= this.HandleConnectedClient;
            this.connection.OnClientDisconnected -= this.HandleDisconnectedClient;
            this.connection.OnReceivedData -= HandleReceivedData;
        }

        private void Send(EventMessage eventData, IEnumerable<Socket> sockets)
        {
            var bytes = EventMessageConvertor.GetBytesForSocket(eventData);
            foreach (var socket in sockets)
            {
                socket.Send(bytes);
            }
        }

        private void HandleConnectedClient(object sender, Socket e)
        {
            this.clients.Add(e);
        }

        private void HandleDisconnectedClient(object sender, Socket e)
        {
            this.clients.Remove(e);
        }

        private void HandleReceivedData(object sender, EventSourceReceiveEventArg<EventMessage> e)
        {
            this.Send(e.EventData, this.clients.Where(socket => socket != e.Client));
        }
    }
}