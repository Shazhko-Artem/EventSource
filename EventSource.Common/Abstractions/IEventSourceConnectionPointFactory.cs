using EventSource.Common.Models;

namespace EventSource.Common.Abstractions
{
    public interface IEventSourceConnectionPointFactory
    {
        EventSourceConnectionPoint Create();
    }
}