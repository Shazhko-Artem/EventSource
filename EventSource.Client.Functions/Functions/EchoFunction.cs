using Azure.WebJobs.Extensions.EventSource;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Azure.WebJobs.Extensions.EventSource.Attributes;

namespace EventSource.Client.Functions.Functions
{
    public class EchoFunction
    {
        private readonly IScopeInvestigator investigator;

        public EchoFunction(IScopeInvestigator scopeInvestigator)
        {
            this.investigator = scopeInvestigator;
        }

        [FunctionName("echo")]
        public async Task<IActionResult> Echo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req,
            [EventSource(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, ConfigurationSectionNames.EventSource.Connection)] IAsyncCollector<EventMessage> eventSource,
            ILogger log)
        {
            var dateTime = DateTime.Now;
            var message= new StringEventMessage("echo", $"Get echo at {dateTime:G}");
            log.LogInformation($"[LOG] Get echo at {dateTime:G}");
            await eventSource.AddAsync(message);
            return new OkResult();
        }
    }
}