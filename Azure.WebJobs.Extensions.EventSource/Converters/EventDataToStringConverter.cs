using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class EventDataToStringConverter : IConverter<EventMessage, string>
    {
        public string Convert(EventMessage input)
        {
            return EventMessageConvertor.GetMessageDataAsString(input);
        }
    }
}