using EventSource.Common.Options;
using EventSource.Server.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventSource.Common;
using EventSource.Common.Abstractions;

namespace EventSource.Server.Web.Extensions
{
    public static class EventSourceServiceRegistrationExtension
    {
        public static IServiceCollection AddEventSource(this IServiceCollection services, Action<EventSourceConnectionOptions> setupOption)
        {
            services.Configure<EventSourceConnectionOptions>(setupOption);
            services.AddSingleton<IConnectionEndPointParser, ConnectionEndPointParser>();
            services.AddSingleton<IEventSourceServer, EventSourceServer>();

            return services;
        }

        public static IApplicationBuilder UseEvenSource(this IApplicationBuilder app)
        {
            var sourceServer = app.ApplicationServices.GetService<IEventSourceServer>();
            sourceServer.Start();

            return app;
        }
    }
}