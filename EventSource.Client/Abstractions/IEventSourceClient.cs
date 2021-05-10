using EventSource.Common.Models.Messages;
using System;

namespace EventSource.Client.Abstractions
{
    public interface IEventSourceClient : IDisposable
    {
        event EventHandler<EventMessage> OnReceivedMessage;

        void Connect();

        void Disconnect();

        void Send(EventMessage message);
    }
}