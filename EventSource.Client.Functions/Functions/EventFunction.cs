using System.Threading.Tasks;
using Azure.WebJobs.Extensions.EventSource;
using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EventSource.Client.Functions.Functions
{
    public class EventFunction
    {
        private readonly IStore<EventMessage> store;

        public EventFunction(IStore<EventMessage> store)
        {
            this.store = store;
        }

        [FunctionName("Events")]
        public IActionResult Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            return new OkObjectResult(this.store.GetAll());
        }

        [FunctionName("Event")]
        public async Task<IActionResult> Send(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] CustomEventMessage<JObject> message,
            [EventSource(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, ConfigurationSectionNames.EventSource.Connection)] IAsyncCollector<EventMessage> eventSource)
        {
            await eventSource.AddAsync(message);
            return new OkResult();
        }
    }
}