using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    internal class EventMessageToPocoConverter<TElement> : IConverter<EventMessage, TElement>
    {
        public TElement Convert(EventMessage message)
        {
            var test = EventMessageConvertor.GetObjectFromMessage<TElement>(message);
            return test;
        }
    }
}