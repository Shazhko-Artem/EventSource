using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Collections.Generic;

namespace Azure.WebJobs.Extensions.EventSource.Triggers
{
    public class EventTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        public override string GetTriggerReason(IDictionary<string, string> arguments)
        {
            return string.Format($"Event message detected at {DateTime.Now}");
        }
    }
}