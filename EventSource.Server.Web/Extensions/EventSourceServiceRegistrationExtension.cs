using EventSource.Common.Abstractions;
using EventSource.Server.Abstractions;
using EventSource.Server.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventSource.Server.Web.Extensions
{
    public static class EventSourceServiceRegistrationExtension
    {
        public static IServiceCollection AddEventSource(this IServiceCollection services, Action<EventSourceConnectionPointOptions> setupOption)
        {
            services.Configure<EventSourceConnectionPointOptions>(setupOption);
            services.AddSingleton<IEventSourceConnectionPointFactory, EventSourceConnectionPointFactory>();
            services.AddSingleton<IEventSourceConnection, EventSourceConnection>();
            services.AddSingleton<IEventSourceServer, EventSourceServer>();

            return services;
        }

        public static IApplicationBuilder UseEvenSource(this IApplicationBuilder app)
        {
            var eventSourceConnection = app.ApplicationServices.GetService<IEventSourceConnection>();
            var sourceServer= app.ApplicationServices.GetService<IEventSourceServer>(); // this is needed to bind the event from the connection to the event source server
            eventSourceConnection.Open();

            return app;
        }
    }
}