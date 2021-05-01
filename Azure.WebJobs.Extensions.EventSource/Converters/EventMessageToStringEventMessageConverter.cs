using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class EventMessageToStringEventMessageConverter : IConverter<EventMessage, StringEventMessage>
    {
        public StringEventMessage Convert(EventMessage message)
        {
            var element = EventMessageConvertor.GetMessageDataAsString(message);
            return new StringEventMessage(message.Name, element);
        }
    }
}