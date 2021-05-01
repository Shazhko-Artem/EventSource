using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class EventMessageToCustomEventMessageConverter<TElement> : IConverter<EventMessage, CustomEventMessage<TElement>>
    {
        public CustomEventMessage<TElement> Convert(EventMessage message)
        {
            var element = EventMessageConvertor.GetObjectFromMessage<TElement>(message);
            return new CustomEventMessage<TElement>(message.Name, element);
        }
    }
}