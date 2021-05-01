using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace Azure.WebJobs.Extensions.EventSource.Attributes
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class EventSourceAttribute : Attribute, IConnectionProvider
    {
        public EventSourceAttribute(string eventName)
        {
            this.EventName = eventName;
        }

        public EventSourceAttribute(string eventName, string connection)
        {
            this.EventName = eventName;
            this.Connection = connection;
        }

        public string Connection { get; set; }

        public string EventName { get; set; }
    }
}