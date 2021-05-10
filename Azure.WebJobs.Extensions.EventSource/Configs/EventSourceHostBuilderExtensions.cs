using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Common.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventSource.Common;
using EventSource.Common.Abstractions;

namespace Azure.WebJobs.Extensions.EventSource.Configs
{
    public static class EventSourceHostBuilderExtensions
    {
        public static IWebJobsBuilder AddEventSource(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddEventSource(p => { });

            return builder;
        }

        public static IWebJobsBuilder AddEventSource(this IWebJobsBuilder builder, Action<EventSourceConnectionOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<EventSourceExtensionConfigProvider>()
                .ConfigureOptions<EventSourceConnectionOptions>((config, path, options) =>
                {
                    options.ConnectionString = config.GetConnectionString(Constants.DefaultConnectionStringName) ??
                                               config[Constants.DefaultConnectionSettingStringName];

                    IConfigurationSection section = config.GetSection(path);
                    section.Bind(options);

                    configure(options);
                });

            builder.Services.AddSingleton<IConnectionEndPointParser, ConnectionEndPointParser>();
            builder.Services.AddSingleton<IEventSourceClientProvider, EventSourceClientProvider>();

            return builder;
        }
    }
}