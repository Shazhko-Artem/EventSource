using EventSource.Common.Models;
using System;
using EventSource.Common.Models.Messages;

namespace EventSource.Client.Abstractions
{
    public interface IEventSourceClient : IDisposable
    {
        event EventHandler<EventMessage> OnReceived;

        void Send(EventMessage data);
    }
}