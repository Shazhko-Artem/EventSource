namespace EventSource.Common.Models.Messages
{
    public class BytesEventMessage : EventMessage
    {
        public BytesEventMessage(string name, byte[] body, string contentType)
        {
            base.Name = name;
            base.Body = body;
            base.ContentType = contentType;
        }
    }
}