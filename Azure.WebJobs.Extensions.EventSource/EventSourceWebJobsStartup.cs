using Azure.WebJobs.Extensions.EventSource;
using Azure.WebJobs.Extensions.EventSource.Configs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(EventSourceWebJobsStartup))]
namespace Azure.WebJobs.Extensions.EventSource
{
    public class EventSourceWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddEventSource();
        }
    }
}