using System;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;

namespace EventSource.Server.Abstractions
{
    public interface IEventSourceServer : IDisposable
    {
        int ClientsCount { get; }

        void Send(EventMessage data);
    }
}