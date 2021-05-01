using EventSource.Common.Abstractions;
using EventSource.Common.Models;
using EventSource.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EventSource.Client
{
    public class EventListener : IEventListener
    {
        private readonly IEventSourceConnectionPointFactory connectionFactory;
        private readonly ILogger<EventListener> logger;
        private readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private volatile bool isStarted = false;

        public EventListener(
            IEventSourceConnectionPointFactory connectionFactory,
            ILogger<EventListener> logger)
        {
            this.connectionFactory = connectionFactory;
            this.logger = logger;
        }


        public event IEventListener.EventHandler EventReceived;

        public void Start()
        {
            if (this.isStarted)
            {
                throw new InvalidOperationException("The event listener has already been started.");
            }

            var connection = this.connectionFactory.Create();
            var client = connection.Socket;
            client.BeginConnect(connection.EndPoint, ConnectCallback, client);
            this.connectDone.WaitOne();
            this.BindReceiveHandler(client);
            this.isStarted = true;
        }

        public void Stop()
        {
            this.isStarted = false;
        }

        public void Dispose()
        {
            this.Stop();
            this.EventReceived = null;
        }

        private void BindReceiveHandler(Socket client)
        {
            try
            {
                StateObject state = new StateObject { workSocket = client };
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to bind receive.");
                this.logger.LogError(e, e.Message);
                throw;
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = client.EndReceive(ar);

                if (state.readBytesCount == 0 && state.bytesCount == 0)
                {
                    state.readBytesCount = bytesRead - sizeof(int);
                    state.bytesCount = BitConverter.ToInt32(state.buffer);
                    state.DataBuilder.Append(Encoding.ASCII.GetString(state.buffer, sizeof(int), bytesRead - sizeof(int)));
                }
                else
                {
                    state.DataBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                }

                if (state.readBytesCount == state.bytesCount)
                {
                    if (this.isStarted)
                    {
                        this.BindReceiveHandler(client);
                    }

                    var eventData = JsonConvert.DeserializeObject<EventData>(state.DataBuilder.ToString());
                    this.EventReceived?.Invoke(eventData);
                }
                else
                {
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
                }

            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to receive.");
                this.logger.LogError(e, e.Message);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);

                this.logger.LogError($"Socket connected to {client.RemoteEndPoint}");
                connectDone.Set();
            }
            catch (Exception e)
            {
                this.logger.LogError("Failed to connect.");
                this.logger.LogError(e, e.Message);
                throw;
            }
        }

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 256;
            public byte[] buffer = new byte[BufferSize];
            public int readBytesCount = 0;
            public int bytesCount = 0;
            public StringBuilder DataBuilder = new StringBuilder();
        }
    }
}