namespace EventSource.Client.Web.Services
{
    public interface IEventSourceWatcher
    {
        void Run();

        void Stop();
    }
}