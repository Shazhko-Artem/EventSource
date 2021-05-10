using System;

namespace EventSource.Server.Models
{
    public class EventSourceReceiveEventArg<TEventData> : EventArgs
    {
        public EventSourceReceiveEventArg(string clientId, TEventData eventData)
        {
            this.ClientId = clientId;
            this.EventData = eventData;
        }

        public string ClientId { get; }

        public TEventData EventData { get; }
    }
}