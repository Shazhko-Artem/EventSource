using EventSource.Client.Abstractions;
using EventSource.Common.Models;
using System;
using System.Collections.Generic;
using EventSource.Common.Models.Messages;

namespace EventSource.Client
{
    public class EventSourceClient : IEventSourceClient
    {
        private readonly IEventSourceConnection connection;
        private readonly List<EventHandler<EventMessage>> receivedHandlers = new List<EventHandler<EventMessage>>();

        public EventSourceClient(IEventSourceConnection connection)
        {
            this.connection = connection;
        }

        public event EventHandler<EventMessage> OnReceived
        {
            add
            {
                this.connection.OnReceivedData += value;
                this.receivedHandlers.Add(value);
            }
            remove
            {
                this.connection.OnReceivedData -= value;
                this.receivedHandlers.Remove(value);
            }
        }

        public void Send(EventMessage data)
        {
            this.connection.Send(data);
        }

        public void Dispose()
        {
            foreach (var handler in receivedHandlers)
            {
                this.connection.OnReceivedData -= handler;
            }
        }
    }
}