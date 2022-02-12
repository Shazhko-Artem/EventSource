using EventSource.Client.Abstractions;
using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Extensions.Logging;
using SimpleTcp;
using System;
using System.Threading.Tasks;
using EventSource.Common.Abstractions;
using EventSource.Common.Options;
using Microsoft.Extensions.Options;

namespace EventSource.Client
{
    public class EventSourceClient : IEventSourceClient
    {
        private readonly ILogger<EventSourceClient> logger;
        private readonly SimpleTcpClient tcpClient;

        public EventSourceClient(
            IOptions<EventSourceConnectionOptions> options,
            IConnectionEndPointParser endPointParser,
            ILogger<EventSourceClient> logger)
        {
            this.logger = logger;
            var value = options.Value;
            var endPoint = endPointParser.Parse(value.ConnectionString);
            this.tcpClient = new SimpleTcpClient(endPoint.Address.ToString(), endPoint.Port);
            this.tcpClient.Events.DataReceived += this.OnReceived;
            this.tcpClient.Events.Connected += this.OnConnected;
            this.tcpClient.Events.Disconnected += this.OnDisconnected;
        }

        public event EventHandler<EventMessage> OnReceivedMessage;

        public void Connect()
        {
            this.logger.LogDebug($"[Connect] Start executing method.");

            if (this.tcpClient.IsConnected)
            {
                this.logger.LogDebug($"[Connect] End executing method.");
                throw new InvalidOperationException("The client has already been connected.");
            }

            this.tcpClient.ConnectWithRetries(timeoutMs: 5000);
            this.logger.LogDebug($"[Connect] End executing method.");
        }

        public void Disconnect()
        {
            this.logger.LogDebug($"[Disconnect] Start executing method.");

            if (this.tcpClient.IsConnected)
            {
                this.logger.LogDebug($"[Disconnect] End executing method.");
                throw new InvalidOperationException("The client has already been connected.");
            }

            this.tcpClient.Disconnect();
            this.logger.LogDebug($"[Disconnect] End executing method.");
        }

        public void Send(EventMessage message)
        {
            this.logger.LogDebug($"[Send] Start executing method.");
            this.logger.LogDebug($"[Send] Serialize '{message.Name}' message into bytes.");
            var bytes = EventMessageConvertor.Serialize(message);
            this.tcpClient.Send(bytes);
            this.logger.LogDebug($"[Send] End executing method.");
        }

        public void Dispose()
        {
            this.tcpClient.Events.DataReceived -= this.OnReceived;
            this.tcpClient.Events.Connected -= this.OnConnected;
            this.tcpClient.Events.Disconnected -= this.OnDisconnected;
            this.tcpClient.Dispose();
        }

        protected virtual void OnReceived(object sender, DataReceivedEventArgs e)
        {
            this.logger.LogDebug("[OnReceived] Start executing method.");
            Task.Run(() =>
            {
                this.logger.LogDebug($"[OnReceived] Deserialize bytes into '{nameof(EventMessage)}' model.");
                var message = EventMessageConvertor.Deserialize<BytesEventMessage>(e.Data);
                this.logger.LogDebug($"[OnReceived] Async invoke subscriber methods to handle received '{message.Name}' message.");
                this.OnReceivedMessage?.Invoke(this, message);
                this.logger.LogDebug("[OnReceived] Finished async invoke executing subscribers.");
            });

            this.logger.LogDebug("[OnReceived] End executing method.");
        }

        protected void OnDisconnected(object sender, ConnectionEventArgs args)
        {
            this.logger.LogDebug($"[OnDisconnected] Disconnected from the server. Client: '{args.IpPort}'");
            if (args.Reason == DisconnectReason.Kicked) return;
            this.logger.LogDebug($"[OnDisconnected] Try reconnect to the server. Client: '{args.IpPort}'");
            this.tcpClient.ConnectWithRetries(timeoutMs: 5000);
            this.logger.LogDebug($"[OnDisconnected] Reconnected to the server. Client: '{args.IpPort}'");
        }

        protected void OnConnected(object sender, ConnectionEventArgs args)
        {
            this.logger.LogDebug($"[OnConnected] Connected to the server. Client: '{args.IpPort}'");
        }
    }
}