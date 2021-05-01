namespace EventSource.Common.Models.Messages
{
    public abstract class EventMessage
    {
        public virtual string Name { get; protected set; }

        public virtual byte[] Body { get; protected set; }

        public virtual string ContentType { get; protected set; }
    }
}