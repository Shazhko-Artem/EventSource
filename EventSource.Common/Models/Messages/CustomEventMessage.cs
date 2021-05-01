using System.Net.Mime;
using EventSource.Common.Convertors;

namespace EventSource.Common.Models.Messages
{
    public class CustomEventMessage<TContent> : EventMessage
    {
        public CustomEventMessage(string name, TContent content)
        {
            base.Name = name;
            this.Content = content;
        }

        public override byte[] Body => EventMessageConvertor.Serialize(this.Content);

        public override string ContentType => MediaTypeNames.Application.Json;

        public TContent Content { get; set; }
    }
}