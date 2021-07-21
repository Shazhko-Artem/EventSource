using System;
using System.Threading.Tasks;
using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace EventSource.Client.Functions.Functions
{
    public class ActivityFunctions
    {
        [FunctionName(nameof(GenerateReport))]
        public async Task<IActionResult> GenerateReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] Activity activity,
            [EventSource(eventName: "newReport", ConfigurationSectionNames.EventSource.Connection)] IAsyncCollector<Report> reportCollector)
        {
            var report = new Report { Name = activity.Name, Date = DateTime.Now };
            await reportCollector.AddAsync(report);
            return new OkResult();
        }
    }
}