using EventSource.Client.Abstractions;
using EventSource.Common.Models.Messages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EventSource.Client.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IEventSourceClient sourceClient;

        public ClientsController(IEventSourceClient sourceClient)
        {
            this.sourceClient = sourceClient;
        }

        [HttpPost("notify")]
        public IActionResult Notify(CustomEventMessage<JObject> message)
        {
            this.sourceClient.Send(message);
            return this.Ok();
        }
    }
}