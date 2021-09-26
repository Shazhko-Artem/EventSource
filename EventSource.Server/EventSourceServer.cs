using EventSource.Common.Abstractions;
using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using EventSource.Common.Options;
using EventSource.Server.Abstractions;
using EventSource.Server.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSource.Server
{
    public class EventSourceServer : IEventSourceServer
    {
        private readonly ILogger<EventSourceServer> logger;
        private readonly SimpleTcpServer tcpServer;

        public EventSourceServer(
            IOptions<EventSourceConnectionOptions> options,
            IConnectionEndPointParser endPointParser,
            ILogger<EventSourceServer> logger)
        {
            this.logger = logger;
            var value = options.Value;
            var endPoint = endPointParser.Parse(value.ConnectionString);
            this.tcpServer = new SimpleTcpServer(endPoint.Address.ToString(), endPoint.Port);
            this.tcpServer.Events.DataReceived += this.OnReceived;
            this.tcpServer.Events.ClientConnected += this.OnClientConnected;
            this.tcpServer.Events.ClientDisconnected += this.OnClientDisconnected;
        }

        public event EventHandler<EventSourceReceiveEventArg<EventMessage>> OnReceivedMessage;

        public int ClientsCount => this.tcpServer.GetClients().Count();

        public void Start()
        {
            this.logger.LogDebug($"[Start] Start executing method.");

            if (this.tcpServer.IsListening)
            {
                this.logger.LogDebug($"[Start] End executing method.");
                throw new InvalidOperationException("The server has already been started.");
            }

            this.tcpServer.Keepalive.EnableTcpKeepAlives = true;
            this.tcpServer.Keepalive.TcpKeepAliveInterval = 3;
            this.tcpServer.Start();
            this.logger.LogDebug($"[Start] End executing method.");
        }

        public void Stop()
        {
            this.logger.LogDebug($"[Stop] Start executing method.");

            if (!this.tcpServer.IsListening)
            {
                this.logger.LogDebug($"[Stop] End executing method.");
                throw new InvalidOperationException("The server has already been stopped.");
            }

            this.tcpServer.Stop();
            this.logger.LogDebug($"[Open] End executing method.");
        }

        public void SendToAll(EventMessage message)
        {
            this.logger.LogDebug($"[SendToAll] Start executing method.");
            this.logger.LogDebug($"[SendToAll] Serialize '{message.Name}' message into bytes.");
            var bytes = EventMessageConvertor.Serialize(message);
            this.logger.LogDebug($"[SendToAll] Get all clients from TcpServer.");
            var clients = this.tcpServer.GetClients();
            this.Send(bytes, clients);
            this.logger.LogDebug($"[SendToAll] End executing method.");
        }

        public void SendToOthers(EventMessage message, string clientId)
        {
            this.logger.LogDebug($"[SendToOthers] Start executing method.");
            this.logger.LogDebug($"[SendToOthers] Serialize '{message.Name}' message into bytes.");
            this.SendTo(message, this.tcpServer.GetClients().Where(id => id != clientId));
            this.logger.LogDebug($"[SendToOthers] End executing method.");
        }

        public void SendTo(EventMessage message, string clientId)
        {
            this.logger.LogDebug($"[SendTo] Start executing method.");
            this.logger.LogDebug($"[SendTo] Serialize '{message.Name}' message into bytes.");
            var bytes = EventMessageConvertor.Serialize(message);
            this.tcpServer.SendAsync(clientId, bytes);
            this.logger.LogDebug($"[SendTo] End executing method.");
        }

        public void SendTo(EventMessage message, IEnumerable<string> clientIds)
        {
            this.logger.LogDebug($"[SendTo] Start executing method.");
            this.logger.LogDebug($"[SendTo] Serialize '{message.Name}' message into bytes.");
            var bytes = EventMessageConvertor.Serialize(message);
            this.Send(bytes, clientIds);
            this.logger.LogDebug($"[SendTo] End executing method.");
        }

        public void Close()
        {
            this.logger.LogDebug("[Close] Start executing method.");
            if (this.tcpServer.IsListening)
            {
                this.logger.LogDebug("[Close] Stop TcpServer.");
                this.tcpServer.Stop();
            }

            this.logger.LogDebug("[Close] End executing method.");
        }

        public void Dispose()
        {
            this.logger.LogDebug("[Dispose] Start executing method.");
            this.Close();
            this.tcpServer.Events.DataReceived -= this.OnReceived;
            this.tcpServer.Events.ClientConnected -= this.OnClientConnected;
            this.tcpServer.Events.ClientDisconnected -= this.OnClientDisconnected;
            this.tcpServer.Dispose();
            this.logger.LogDebug("[Dispose] End executing method.");
        }

        protected virtual void OnReceived(object sender, DataReceivedEventArgs e)
        {
            this.logger.LogDebug("[OnReceived] Start executing method.");
            this.logger.LogDebug($"[OnReceived] Deserialize bytes into '{nameof(EventMessage)}' model.");
            var message = EventMessageConvertor.Deserialize<BytesEventMessage>(e.Data);
            this.logger.LogDebug($"[OnReceived] Received '{message.Name}' message from the bytes.");
            this.SendToOthers(message, e.IpPort);
            this.logger.LogDebug($"[OnReceived] Invoke subscriber methods to handle received '{message.Name}' message.");
            this.OnReceivedMessage?.Invoke(this, new EventSourceReceiveEventArg<EventMessage>(e.IpPort, message));
            this.logger.LogDebug("[OnReceived] End executing method.");
        }

        protected void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            this.logger.LogInformation($"[OnClientConnected] New client connected. Client '{e.IpPort}'");
        }

        protected void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.logger.LogInformation($"[OnClientDisconnected] Client disconnected. Client: '{e.IpPort}', Reason: {e.Reason.ToString()}");
        }
        
        private void Send(byte[] data, IEnumerable<string> clientIds)
        {
            foreach (var clientId in clientIds)
            {
                this.logger.LogDebug($"[Send] Send message to the '{clientId}' client.");
                this.tcpServer.SendAsync(clientId, data);
            }
        }
    }
}