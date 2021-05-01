using EventSource.Client.Abstractions;
using EventSource.Client.Options;
using EventSource.Client.Web.Services;
using EventSource.Common.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventSource.Client.Web.Services.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSource.Client.Web.Extensions
{
    public static class EventSourceServiceRegistrationExtension
    {
        public static IServiceCollection AddEventSourceClient(this IServiceCollection services, Action<EventSourceConnectionOptions> setupOption)
        {
            services.Configure<EventSourceConnectionOptions>(setupOption);
            services.AddSingleton<IEventSourceConnectionPointFactory>(provider =>
            {
                var options = provider.GetService<IOptions<EventSourceConnectionOptions>>();
                var logger = provider.GetService<ILogger<EventSourceConnectionPointFactory>>();
                return new EventSourceConnectionPointFactory(options.Value.ConnectionString, logger);
            });
            services.AddSingleton<IEventSourceConnection, EventSourceConnection>();
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
            var eventSourceConnection = app.ApplicationServices.GetService<IEventSourceConnection>();
            eventSourceConnection.Open();

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