using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventSource.Client.Functions.Functions
{
    public class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public void Run([ServiceBusTrigger("myqueue", Connection = "")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }

        //[FunctionName("echo")]
        //public IActionResult Echo(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req, 
        //    ILogger log)
        //{
        //    var dateTime = DateTime.Now;
        //    log.LogInformation($"[LOG] Get echo at {dateTime:G}");
        //    return new OkResult();
        //}
    }
}
