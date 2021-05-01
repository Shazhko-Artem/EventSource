using EventSource.Client.Abstractions;
using EventSource.Common.Models;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSource.Common.Models.Messages;

namespace Azure.WebJobs.Extensions.EventSource.Bindings
{
    internal class EventSourceAsyncCollector : IAsyncCollector<EventMessage>
    {
        private readonly string eventName;
        private readonly IEventSourceClient client;
        private readonly List<EventMessage> batch = new List<EventMessage>();

        public EventSourceAsyncCollector(string eventName, IEventSourceClient client)
        {
            this.eventName = eventName;
            this.client = client;
        }

        public Task AddAsync(EventMessage data, CancellationToken cancellationToken = new CancellationToken())
        {
            var clone = new BytesEventMessage(data.Name ?? this.eventName, data.Body.ToArray(), data.ContentType);
            this.batch.Add(clone);

            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            this.batch.ForEach(data => this.client.Send(data));
            this.batch.Clear();

            return Task.CompletedTask;
        }
    }
}