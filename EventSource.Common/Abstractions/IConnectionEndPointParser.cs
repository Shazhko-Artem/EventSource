using System.Net;

namespace EventSource.Common.Abstractions
{
    public interface IConnectionEndPointParser
    {
        IPEndPoint Parse(string connectionString);
    }
}