using EventSource.Client.Abstractions;
using EventSource.Common.Abstractions;
using EventSource.Common.Communication;
using EventSource.Common.Convertors;
using EventSource.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using EventSource.Common.Models.Messages;

namespace EventSource.Client
{
    public class EventSourceConnection : BasicSocketCommunication<BytesEventMessage>, IEventSourceConnection
    {
        private readonly IEventSourceConnectionPointFactory connectionPointFactory;
        private readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private Socket socketClient;

        public EventSourceConnection(
            IEventSourceConnectionPointFactory connectionPointFactory,
            ILogger<EventSourceConnection> logger) : base(logger)
        {
            this.connectionPointFactory = connectionPointFactory;
        }

        public event EventHandler<EventMessage> OnReceivedData;

        public void Open()
        {
            if (this.IsOpened)
            {
                throw new InvalidOperationException("The event source connection has already been opened.");
            }

            var connection = this.connectionPointFactory.Create();
            this.socketClient = connection.Socket;
            socketClient.BeginConnect(connection.EndPoint, ConnectCallback, socketClient);
            this.connectDone.WaitOne();
            this.BindReceiveHandler(socketClient);
            this.IsOpened = true;
        }

        public void Send(EventMessage data)
        {
            var bytes = EventMessageConvertor.GetBytesForSocket(data);
            this.socketClient.Send(bytes);
        }

        public void Stop()
        {
            if (!this.IsOpened)
            {
                throw new InvalidOperationException("The event source connection has already been stopped.");
            }

            this.IsOpened = false;
        }

        public void Dispose()
        {
            this.Stop();
            this.OnReceivedData = null;
            this.socketClient = null;
        }

        protected override void OnReceived(Socket client, BytesEventMessage eventData)
        {
            this.OnReceivedData?.Invoke(this, eventData);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;
                client.EndConnect(ar);

                this.Logger.LogInformation($"Socket connected to '{client.RemoteEndPoint}'");
                connectDone.Set();
            }
            catch (Exception e)
            {
                this.Logger.LogError("Failed to connect.");
                this.Logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}