using Azure.WebJobs.Extensions.EventSource.Attributes;
using Azure.WebJobs.Extensions.EventSource.Bindings;
using Azure.WebJobs.Extensions.EventSource.Converters;
using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using Azure.WebJobs.Extensions.EventSource.Triggers;
using EventSource.Client.Abstractions;
using EventSource.Common.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using EventSource.Common.Models.Messages;
using EventSource.Common.Options;
using Microsoft.Azure.WebJobs.Host;

namespace Azure.WebJobs.Extensions.EventSource.Configs
{
    [Extension("EventSource")]
    internal class EventSourceExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly INameResolver nameResolver;
        private readonly IEventSourceClientProvider clientProvider;
        private readonly IConfiguration configuration;
        private readonly EventSourceConnectionOptions options;

        public EventSourceExtensionConfigProvider(
            INameResolver nameResolver,
            IEventSourceClientProvider clientProvider,
            IConfiguration configuration,
            IOptions<EventSourceConnectionOptions> options)
        {
            this.nameResolver = nameResolver;
            this.clientProvider = clientProvider;
            this.configuration = configuration;
            this.options = options.Value;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context
                .AddBindingRule<EventSourceAttribute>()
                .AddOpenConverter<OpenType.Poco, EventMessage>(typeof(PocoToEventMessageConverter<>))
                .BindToCollector<EventMessage>(attribute =>
                {
                    var eventName = nameResolver.ResolveWholeString(attribute.EventName);
                    return new EventSourceAsyncCollector(eventName, this.GetEventSourceClient(attribute));
                });

            var triggerBindingProvider = new EventSourceTriggerAttributeBindingProvider(
                this.clientProvider, this.configuration, this.options, this.nameResolver);

            context
                .AddBindingRule<EventSourceTriggerAttribute>()
                .AddConverter<EventMessage, DirectInvokeString>(new EventDataToDirectInvokeStringConverter())
                .AddConverter<EventMessage, BytesEventMessage>(new EventDataToBytesEventMessageConverter())
                .AddConverter<EventMessage, StringEventMessage>(new EventMessageToStringEventMessageConverter())
                .AddConverter<EventMessage, string>(new EventDataToStringConverter())
                .AddOpenConverter<EventMessage, CustomEventMessage<OpenType.Poco>>(typeof(EventMessageToCustomEventMessageConverter<>))
                .AddOpenConverter<EventMessage, OpenType.Poco>(typeof(EventMessageToPocoConverter<>))
                .BindToTrigger<EventMessage>(triggerBindingProvider);

        }

        private IEventSourceClient GetEventSourceClient(EventSourceAttribute attribute)
        {
            var account = new EventSourceAccount(this.options, this.configuration, attribute);
            return this.clientProvider.GetClient(account);
        }
    }
}