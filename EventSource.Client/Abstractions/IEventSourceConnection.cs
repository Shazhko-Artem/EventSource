using EventSource.Common.Models;
using System;
using EventSource.Common.Models.Messages;

namespace EventSource.Client.Abstractions
{
    public interface IEventSourceConnection : IDisposable
    {
        event EventHandler<EventMessage> OnReceivedData;

        void Open();

        void Send(EventMessage data);
    }
}