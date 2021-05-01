using System;
using System.Net.Sockets;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;
using EventSource.Server.Models;

namespace EventSource.Server.Abstractions
{
    public interface IEventSourceConnection : IDisposable
    {
        event EventHandler<Socket> OnClientConnected;

        event EventHandler<Socket> OnClientDisconnected;

        event EventHandler<EventSourceReceiveEventArg<EventMessage>> OnReceivedData;
        
        void Open();
    }
}