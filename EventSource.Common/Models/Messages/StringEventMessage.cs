using EventSource.Common.Convertors;
using System.Net.Mime;

namespace EventSource.Common.Models.Messages
{
    public class StringEventMessage : EventMessage
    {
        public StringEventMessage(string name, string content)
        {
            base.Name = name;
            this.Content = content;
        }

        public override byte[] Body => EventMessageConvertor.GetEncoding().GetBytes(this.Content);

        public override string ContentType => MediaTypeNames.Text.Plain;

        public string Content { get; }
    }
}