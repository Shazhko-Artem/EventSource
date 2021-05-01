using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class PocoToEventMessageConverter<TElement> : IConverter<TElement, EventMessage>
    {
        public EventMessage Convert(TElement element)
        {
            return new CustomEventMessage<TElement>(name: null, element);
        }
    }
}