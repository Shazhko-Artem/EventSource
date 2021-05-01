using EventSource.Client.Web.Services;
using EventSource.Common.Models.Messages;
using Microsoft.AspNetCore.Mvc;

namespace EventSource.Client.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IStore<EventMessage> store;

        public EventsController(IStore<EventMessage> store)
        {
            this.store = store;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok(this.store.GetAll());
        }
    }
}