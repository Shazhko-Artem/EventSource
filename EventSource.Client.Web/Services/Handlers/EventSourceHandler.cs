using System.Net.Mime;
using EventSource.Common.Convertors;
using EventSource.Common.Models.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EventSource.Client.Web.Services.Handlers
{
    public class EventSourceHandler : IEventSourceHandler
    {
        private readonly IStore<EventMessage> store;
        private readonly ILogger<EventSourceHandler> logger;

        public EventSourceHandler(
            IStore<EventMessage> store,
            ILogger<EventSourceHandler> logger)
        {
            this.store = store;
            this.logger = logger;
        }

        public bool CanHandle(EventMessage data)
        {
            return true;
        }

        public void Handle(EventMessage message)
        {
            switch (message.ContentType)
            {
                case MediaTypeNames.Application.Json:
                    {
                        var content = EventMessageConvertor.GetObjectFromMessage<JObject>(message);
                        var fullMessage = new CustomEventMessage<JObject>(message.Name, content);
                        this.store.Add(fullMessage);
                        this.logger.LogInformation($"[EVENT SOURCE] Get event '{message.Name}'. Content: '{fullMessage.Content}'");

                        break;
                    }
                case MediaTypeNames.Text.Plain:
                    {
                        var content = EventMessageConvertor.GetMessageDataAsString(message);
                        var fullMessage = new StringEventMessage(message.Name, content);
                        this.logger.LogInformation($"[EVENT SOURCE] Get event '{message.Name}'. Content: '{fullMessage.Content}'");
                        this.store.Add(fullMessage);
                        break;
                    }
                default:
                    this.logger.LogWarning($"Received message with unknown content type '{message.ContentType}'");
                    return;
            }

        }
    }
}