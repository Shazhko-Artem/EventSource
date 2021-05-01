using Azure.WebJobs.Extensions.EventSource.Configs;
using EventSource.Client.Abstractions;

namespace Azure.WebJobs.Extensions.EventSource.Services.Connection
{
    public interface IEventSourceClientProvider
    {
        IEventSourceClient GetClient(EventSourceAccount account);
    }
}