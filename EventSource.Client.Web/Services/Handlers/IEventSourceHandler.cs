using System.Threading.Tasks;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;

namespace EventSource.Client.Web.Services.Handlers
{
    public interface IEventSourceHandler
    {
        bool CanHandle(EventMessage data);

        void Handle(EventMessage data);
    }
}