using EventSource.Common.Abstractions;
using EventSource.Common.Models;
using EventSource.Server.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EventSource.Common.Communication;
using EventSource.Common.Models.Messages;
using EventSource.Server.Models;
using Microsoft.Extensions.Logging;

namespace EventSource.Server
{
    public class EventSourceConnection : BasicSocketCommunication<BytesEventMessage>, IEventSourceConnection
    {
        private readonly ManualResetEvent allDone = new ManualResetEvent(false);
        private readonly IEventSourceConnectionPointFactory connectionPointFactory;
        private CancellationTokenSource tokenSource;

        public EventSourceConnection(
            IEventSourceConnectionPointFactory connectionPointFactory,
            ILogger<EventSourceConnection> logger) : base(logger)
        {
            this.connectionPointFactory = connectionPointFactory;
        }

        public event EventHandler<Socket> OnClientConnected;

        public event EventHandler<Socket> OnClientDisconnected;

        public event EventHandler<EventSourceReceiveEventArg<EventMessage>> OnReceivedData;

        public void Open()
        {
            if (this.IsOpened)
            {
                throw new InvalidOperationException("The connection has already been created.");
            }

            var connection = this.connectionPointFactory.Create();
            connection.Socket.Bind(connection.EndPoint);
            connection.Socket.Listen(backlog: 100);
            this.tokenSource = new CancellationTokenSource();
            this.StartListening(connection, tokenSource.Token);
            this.IsOpened = true;
            this.Logger.LogInformation($"Event source listening '{connection.EndPoint}'");
        }

        public void Close()
        {
            if (!this.IsOpened)
            {
                return;
            }

            this.tokenSource.Cancel(true);
            this.IsOpened = false;
        }

        public void Dispose()
        {
            this.Logger.LogDebug("Dispose event source connection");
            this.Close();
        }

        protected override void OnReceived(Socket client, BytesEventMessage eventData)
        {
            this.OnReceivedData?.Invoke(this, new EventSourceReceiveEventArg<EventMessage>(client, eventData));
        }

        protected override void OnDisconnected(Socket socketClient)
        {
            this.OnClientDisconnected?.Invoke(this, socketClient);
        }

        private void StartListening(EventSourceConnectionPoint connection, CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    allDone.Reset();
                    connection.Socket.BeginAccept(OnAccept, connection.Socket);
                    allDone.WaitOne();
                }
            }, token);
        }

        private void OnAccept(IAsyncResult asyncResult)
        {
            allDone.Set();

            var listener = (Socket)asyncResult.AsyncState;
            var handler = listener.EndAccept(asyncResult);

            this.Logger.LogInformation($"A new client was connected at '{handler.RemoteEndPoint}'");
            this.OnClientConnected?.Invoke(this, handler);

            this.BindReceiveHandler(handler);
        }
    }
}