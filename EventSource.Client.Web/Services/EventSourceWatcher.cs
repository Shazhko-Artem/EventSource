using EventSource.Client.Abstractions;
using EventSource.Client.Web.Services.Handlers;
using EventSource.Common.Models.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EventSource.Client.Web.Services
{
    public class EventSourceWatcher : IEventSourceWatcher
    {
        private readonly IEventSourceClient eventSourceClient;
        private readonly IServiceProvider provider;
        private readonly ILogger<EventSourceWatcher> logger;

        public EventSourceWatcher(
            IEventSourceClient eventSourceClient,
            IServiceProvider provider,
            ILogger<EventSourceWatcher> logger)
        {
            this.eventSourceClient = eventSourceClient;
            this.provider = provider;
            this.logger = logger;
        }

        public void Run()
        {
            this.logger.LogInformation("Start watching event source");
            eventSourceClient.OnReceivedMessage += this.HandleEvent;
        }

        public void Stop()
        {
            this.logger.LogInformation("Stope watching event source");
            eventSourceClient.OnReceivedMessage -= this.HandleEvent;
        }

        private void HandleEvent(object? sender, EventMessage eventData)
        {
            using var scope = provider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventSourceHandler>();
            foreach (var handler in handlers)
            {
                handler.Handle(eventData);
            }
        }
    }
}