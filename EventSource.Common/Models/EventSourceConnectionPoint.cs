using System.Net;
using System.Net.Sockets;

namespace EventSource.Common.Models
{
    public class EventSourceConnectionPoint
    {
        public EventSourceConnectionPoint(Socket socket, IPEndPoint endPoint)
        {
           this.Socket = socket;
           this.EndPoint = endPoint;
        }

        public Socket Socket { get; }

        public IPEndPoint EndPoint { get; }
    }
}