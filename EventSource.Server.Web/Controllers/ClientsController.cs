using EventSource.Common.Models.Messages;
using EventSource.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EventSource.Server.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IEventSourceServer sourceServer;

        public ClientsController(IEventSourceServer sourceServer)
        {
            this.sourceServer = sourceServer;
        }


        [HttpGet("count")]
        public IActionResult GetCount()
        {
            return this.Ok(this.sourceServer.ClientsCount);
        }

        [HttpPost("notify")]
        public IActionResult Notify([FromBody] CustomEventMessage<JObject> message)
        {
            this.sourceServer.SendToAll(message);
            return this.Ok();
        }
    }
}