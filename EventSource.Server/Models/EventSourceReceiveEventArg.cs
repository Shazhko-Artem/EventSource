using System;
using System.Net.Sockets;

namespace EventSource.Server.Models
{
    public class EventSourceReceiveEventArg<TEventData> : EventArgs
    {
        public EventSourceReceiveEventArg(Socket client, TEventData eventData)
        {
            this.Client = client;
            this.EventData = eventData;
        }

        public Socket Client { get; }

        public TEventData EventData { get; }
    }
}