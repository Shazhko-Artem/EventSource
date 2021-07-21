using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace EventSource.Client.Functions.Functions
{
    public class EventFunction
    {
        private readonly IStore<EventMessage> store;

        public EventFunction(IStore<EventMessage> store)
        {
            this.store = store;
        }

        [FunctionName(nameof(GetEvent))]
        public IActionResult GetEvent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request)
        {
            return new OkObjectResult(this.store.GetAll());
        }

        [FunctionName(nameof(SendEvent))]
        public async Task<IActionResult> SendEvent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] CustomEventMessage<JObject> message,
            [EventSource(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, ConfigurationSectionNames.EventSource.Connection)] IAsyncCollector<EventMessage> eventSource)
        {
            await eventSource.AddAsync(message);
            return new OkResult();
        }
    }
}