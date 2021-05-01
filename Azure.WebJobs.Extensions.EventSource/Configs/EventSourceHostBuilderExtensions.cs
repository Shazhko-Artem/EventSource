using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Client;
using EventSource.Client.Options;
using EventSource.Common.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            builder.Services.AddSingleton<IEventSourceClientProvider, EventSourceClientProvider>();

            return builder;
        }
    }
}