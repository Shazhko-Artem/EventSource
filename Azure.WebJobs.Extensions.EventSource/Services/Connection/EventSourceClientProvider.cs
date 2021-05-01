using Azure.WebJobs.Extensions.EventSource.Configs;
using EventSource.Client;
using EventSource.Client.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Azure.WebJobs.Extensions.EventSource.Services.Connection
{
    public class EventSourceClientProvider : IEventSourceClientProvider
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ConcurrentDictionary<string, IEventSourceClient> clientCache = new ConcurrentDictionary<string, IEventSourceClient>();

        public EventSourceClientProvider(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public IEventSourceClient GetClient(EventSourceAccount account)
        {
            var connectionString = account.ConnectionString;
            return clientCache.GetOrAdd(connectionString, this.CreateClient);
        }

        private IEventSourceClient CreateClient(string connectionString)
        {
            var factoryLogger = this.loggerFactory.CreateLogger<EventSourceConnectionPointFactory>();
            var connectionPointFactory = new EventSourceConnectionPointFactory(connectionString, factoryLogger);

            var connectionLogger = this.loggerFactory.CreateLogger<EventSourceConnection>();
            var connection = new EventSourceConnection(connectionPointFactory, connectionLogger);

            connection.Open();
            var client = new EventSourceClient(connection);
            return client;
        }
    }
}