using Azure.WebJobs.Extensions.EventSource.Configs;
using EventSource.Client;
using EventSource.Client.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using EventSource.Common;
using EventSource.Common.Abstractions;
using EventSource.Common.Options;
using Microsoft.Extensions.Options;

namespace Azure.WebJobs.Extensions.EventSource.Services.Connection
{
    public class EventSourceClientProvider : IEventSourceClientProvider
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConnectionEndPointParser connectionEndPointParser;
        private readonly ConcurrentDictionary<string, IEventSourceClient> clientCache = new ConcurrentDictionary<string, IEventSourceClient>();

        public EventSourceClientProvider(
            ILoggerFactory loggerFactory,
            IConnectionEndPointParser connectionEndPointParser)
        {
            this.loggerFactory = loggerFactory;
            this.connectionEndPointParser = connectionEndPointParser;
        }

        public IEventSourceClient GetClient(EventSourceAccount account)
        {
            var connectionString = account.ConnectionString;
            return clientCache.GetOrAdd(connectionString, this.CreateClient);
        }

        private IEventSourceClient CreateClient(string connectionString)
        {
            var clientLogger = this.loggerFactory.CreateLogger<EventSourceClient>();
            var options = new EventSourceConnectionOptions { ConnectionString = connectionString };
            var optionsWrapper = new OptionsWrapper<EventSourceConnectionOptions>(options);
            var client = new EventSourceClient(optionsWrapper, this.connectionEndPointParser, clientLogger);
            client.Connect();
            return client;
        }
    }
}