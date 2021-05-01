using System;
using EventSource.Client.Abstractions;
using EventSource.Common.Models.Messages;
using Microsoft.Extensions.Logging;

namespace EventSource.Client.Functions.Services
{
    public class ScopeInvestigator : IScopeInvestigator
    {
        private readonly IEventSourceClient client;
        private readonly ILogger<ScopeInvestigator> logger;

        public ScopeInvestigator(IEventSourceClient client, ILogger<ScopeInvestigator> logger)
        {
            this.client = client;
            this.logger = logger;
            this.SendEvent();
        }

        private void SendEvent()
        {
            var date = DateTime.Now;
            this.logger.LogInformation($"[{date:G}] Creating a new instance of the '{nameof(ScopeInvestigator)}' class.");
            this.client.Send(new StringEventMessage("CreatingSingletonObject", $"[{date:G}] Creating a new instance of the '{nameof(ScopeInvestigator)}' class."));
        }

        public void Dispose()
        {
            var date = DateTime.Now;
            this.logger.LogInformation($"[{date:G}] Disposing the instance of the '{nameof(ScopeInvestigator)}' class.");
            this.client.Send(new StringEventMessage("DisposingSingletonObject", $"[{date:G}] Disposing the instance of the '{nameof(ScopeInvestigator)}' class."));

        }
    }
}