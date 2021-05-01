using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace Azure.WebJobs.Extensions.EventSource.Attributes
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EventSourceTriggerAttribute : Attribute, IConnectionProvider
    {
        public EventSourceTriggerAttribute(string eventName)
        {
            this.EventName = eventName;
        }

        public EventSourceTriggerAttribute(string eventName, string connection)
        {
            this.EventName = eventName;
            this.Connection = connection;
        }

        public string Connection { get; set; }

        public string EventName { get; set; }
    }
}