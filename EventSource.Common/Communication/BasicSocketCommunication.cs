using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using EventSource.Common.Convertors;

namespace EventSource.Common.Communication
{
    public abstract class BasicSocketCommunication<TData>
    {
        protected ILogger Logger { get; }
        protected volatile bool IsOpened;

        protected BasicSocketCommunication(ILogger logger)
        {
            this.Logger = logger;
        }

        protected virtual void BindReceiveHandler(Socket client)
        {
            try
            {
                var state = new StateObject(client);
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
            }
            catch (Exception e)
            {
                this.Logger.LogError("Failed to bind receive.");
                this.Logger.LogError(e, e.Message);
                throw;
            }
        }

        protected virtual void OnReceive(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var socketClient = state.WorkSocket;
            if (!socketClient.Connected)
            {
                this.Logger.LogInformation($"Client was disconnected at '{socketClient.RemoteEndPoint}'");
                this.OnDisconnected(socketClient);
                return;
            }

            try
            {
                var bytesRead = socketClient.EndReceive(ar);

                if (state.HasBeenReadBytes == 0 && state.TotalBytesCount == 0)
                {
                    var hasBeenReadBytes = bytesRead - sizeof(int);
                    var totalBytesCount = BitConverter.ToInt32(state.Buffer);
                    state.DataBuilder.Append(EventMessageConvertor.GetEncoding().GetString(state.Buffer, sizeof(int), bytesRead - sizeof(int)));
                    state = new StateObject(socketClient, totalBytesCount, hasBeenReadBytes, state.DataBuilder);
                }
                else
                {
                    state.DataBuilder.Append(EventMessageConvertor.GetEncoding().GetString(state.Buffer, 0, bytesRead));
                    state.HasBeenReadBytes += bytesRead;
                }

                if (state.HasBeenReadBytes == state.TotalBytesCount)
                {
                    if (this.IsOpened)
                    {
                        this.BindReceiveHandler(socketClient);
                    }

                    var eventData = JsonConvert.DeserializeObject<TData>(state.DataBuilder.ToString());
                    this.OnReceived(socketClient, eventData);
                }
                else
                {
                    socketClient.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
                }

            }
            catch (SocketException e)
            {
                this.Logger.LogError("Failed to receive.");
                this.Logger.LogError(e, e.Message);
                if (!socketClient.Connected)
                {
                    this.Logger.LogInformation($"Client was disconnected at '{socketClient.RemoteEndPoint}'");
                    this.OnDisconnected(socketClient);
                }
            }
        }

        protected abstract void OnReceived(Socket socketClient, TData eventData);

        protected virtual void OnDisconnected(Socket socketClient) { }

        public class StateObject
        {
            public StateObject(Socket socket)
            {
                this.WorkSocket = socket;
            }

            public StateObject(Socket socket, in int totalBytesCount, in int hasBeenReadBytes, StringBuilder dataBuilder)
                : this(socket)
            {
                this.TotalBytesCount = totalBytesCount;
                this.HasBeenReadBytes = hasBeenReadBytes;
                this.DataBuilder = dataBuilder;
            }

            public const int BufferSize = 256;

            public byte[] Buffer { get; } = new byte[BufferSize];

            public Socket WorkSocket { get; }

            public int HasBeenReadBytes { get; set; }

            public int TotalBytesCount { get; }

            public StringBuilder DataBuilder { get; } = new StringBuilder();
        }
    }
}