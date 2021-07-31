using Azure.WebJobs.Extensions.EventSource.Attributes;
using Azure.WebJobs.Extensions.EventSource.Configs;
using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.WebJobs.Extensions.EventSource.Triggers
{
    internal class EventSourceTriggerAttributeBinding : ITriggerBinding
    {
        private readonly IEventSourceClientProvider clientProvider;
        private readonly INameResolver nameResolver;
        private readonly EventSourceAccount account;
        private readonly EventSourceTriggerAttribute attribute;

        public EventSourceTriggerAttributeBinding(
            IEventSourceClientProvider clientProvider,
            EventSourceAccount account,
            EventSourceTriggerAttribute attribute,
            INameResolver nameResolver)
        {
            this.clientProvider = clientProvider;
            this.nameResolver = nameResolver;
            this.account = account;
            this.attribute = attribute;
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

            var client = this.clientProvider.GetClient(this.account);
            var eventName = this.nameResolver.ResolveWholeString(attribute.EventName);
            return Task.FromResult<IListener>(new TriggerEventListener(context.Executor, client, eventName));
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