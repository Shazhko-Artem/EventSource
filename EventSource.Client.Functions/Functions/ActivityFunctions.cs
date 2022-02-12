using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;

namespace EventSource.Client.Functions.Functions
{
    public class ActivityFunctions
    {
        [FunctionName(nameof(GenerateReport))]
        public async Task GenerateReport(
            [EventSourceTrigger("newActivity", ConfigurationSectionNames.EventSource.Connection)] Activity activity,
            [EventSource(eventName: "newReport", ConfigurationSectionNames.EventSource.Connection)] IAsyncCollector<Report> reportCollector)
        {
            var report = new Report { Name = activity.Name, Date = DateTime.Now };
            await reportCollector.AddAsync(report);
        }
    }
}