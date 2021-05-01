using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class EventDataToBytesEventMessageConverter : IConverter<EventMessage, BytesEventMessage>
    {
        public BytesEventMessage Convert(EventMessage message)
        {
            return new BytesEventMessage(message.Name, message.Body, message.ContentType);
        }
    }
}