using Azure.WebJobs.Extensions.EventSource.Attributes;
using Azure.WebJobs.Extensions.EventSource.Configs;
using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Client.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Azure.WebJobs.Extensions.EventSource.Triggers
{
    internal class EventSourceTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly IEventSourceClientProvider clientProvider;
        private readonly EventSourceConnectionOptions options;
        private readonly INameResolver nameResolver;
        private readonly IConfiguration configuration;

        public EventSourceTriggerAttributeBindingProvider(
            IEventSourceClientProvider clientProvider,
            IConfiguration configuration,
            EventSourceConnectionOptions options,
            INameResolver nameResolver)
        {
            this.clientProvider = clientProvider;
            this.configuration = configuration;
            this.options = options;
            this.nameResolver = nameResolver;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;
            var attribute = TypeUtility.GetResolvedAttribute<EventSourceTriggerAttribute>(parameter);

            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            var account = new EventSourceAccount(this.options, this.configuration, attribute);
            var client = this.clientProvider.GetClient(account);

            var eventName = this.nameResolver.ResolveWholeString(attribute.EventName);
            return Task.FromResult<ITriggerBinding>(new EventSourceTriggerAttributeBinding(client, eventName));
        }
    }
}