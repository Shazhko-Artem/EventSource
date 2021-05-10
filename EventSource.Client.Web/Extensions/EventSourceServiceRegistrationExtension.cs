using EventSource.Client.Abstractions;
using EventSource.Client.Web.Services;
using EventSource.Client.Web.Services.Handlers;
using EventSource.Common.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventSource.Common;
using EventSource.Common.Abstractions;

namespace EventSource.Client.Web.Extensions
{
    public static class EventSourceServiceRegistrationExtension
    {
        public static IServiceCollection AddEventSourceClient(this IServiceCollection services, Action<EventSourceConnectionOptions> setupOption)
        {
            services.Configure<EventSourceConnectionOptions>(setupOption);
            services.AddSingleton<IConnectionEndPointParser, ConnectionEndPointParser>();
            services.AddSingleton<IEventSourceClient, EventSourceClient>();

            return services;
        }

        public static IServiceCollection AddEventSourceHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IEventSourceWatcher, EventSourceWatcher>();
            services.AddSingleton<IEventSourceHandler, EventSourceHandler>();

            return services;
        }

        public static IApplicationBuilder UseEvenSourceClient(this IApplicationBuilder app)
        {
            var eventSourceConnection = app.ApplicationServices.GetService<IEventSourceClient>();
            eventSourceConnection.Connect();

            return app;
        }

        public static IApplicationBuilder UseEventSourceHandlers(this IApplicationBuilder app)
        {
            var eventSourceWatcher = app.ApplicationServices.GetService<IEventSourceWatcher>();
            eventSourceWatcher.Run();

            return app;
        }
    }
}