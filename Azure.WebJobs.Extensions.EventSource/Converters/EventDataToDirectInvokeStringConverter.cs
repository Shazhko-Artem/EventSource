using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Azure.WebJobs.Extensions.EventSource.Converters
{
    public class EventDataToDirectInvokeStringConverter : IConverter<EventMessage, DirectInvokeString>
    {
        public DirectInvokeString Convert(EventMessage input)
        {
            var value= EventMessageConvertor.GetMessageDataAsString(input);
            return new DirectInvokeString(value);
        }
    }
}