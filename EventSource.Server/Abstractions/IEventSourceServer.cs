using EventSource.Common.Models.Messages;
using EventSource.Server.Models;
using System;
using System.Collections.Generic;

namespace EventSource.Server.Abstractions
{
    public interface IEventSourceServer : IDisposable
    {
        event EventHandler<EventSourceReceiveEventArg<EventMessage>> OnReceivedMessage;

        int ClientsCount { get; }

        void Start();

        void Stop();

        void SendToAll(EventMessage message);

        void SendToOthers(EventMessage message, string fromClientId);
        
        void SendTo(EventMessage message, string clientId);

        void SendTo(EventMessage message, IEnumerable<string> clientIds);
    }
}