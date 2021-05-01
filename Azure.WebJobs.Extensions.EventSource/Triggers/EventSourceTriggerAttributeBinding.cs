using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSource.Client.Abstractions;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Azure.WebJobs.Extensions.EventSource.Triggers
{
    internal class EventSourceTriggerAttributeBinding : ITriggerBinding
    {
        private readonly IEventSourceClient client;
        private readonly string eventName;

        public EventSourceTriggerAttributeBinding(IEventSourceClient client, string eventName)
        {
            this.client = client;
            this.eventName = eventName;
            this.BindingDataContract = this.CreateBindingDataContract();
        }

        public Type TriggerValueType => typeof(EventMessage);

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            var emptyTriggerData = new TriggerData(null, new Dictionary<string, object>());
            return Task.FromResult<ITriggerData>(emptyTriggerData);
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult<IListener>(new TriggerEventListener(context.Executor, this.client, this.eventName));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new EventTriggerParameterDescriptor();
        }

        private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
        {
            var contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                {nameof(EventMessage.Name), typeof(string)},
                {nameof(EventMessage.Body), typeof(string)}
            };


            return contract;
        }
    }
}