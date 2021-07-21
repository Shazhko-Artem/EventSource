using Azure.WebJobs.Extensions.EventSource.Configs;
using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Client.Abstractions;
using EventSource.Client.Functions;
using EventSource.Client.Functions.Services;
using EventSource.Common.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace EventSource.Client.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            services.AddOptions<EventSourceConnectionOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration.GetSection("EventSource").Bind(options));

            services.AddSingleton<IEventSourceClient>(provider =>
            {
                var clientProvider = provider.GetRequiredService<IEventSourceClientProvider>();
                var options = provider.GetService<IOptions<EventSourceConnectionOptions>>().Value;
                var account = new EventSourceAccount(options, configuration: null, connectionProvider: null);
                return clientProvider.GetClient(account);
            });

            services.AddSingleton<IScopeInvestigator, ScopeInvestigator>();
            services.AddSingleton(typeof(IStore<>), typeof(Store<>));
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
        }
    }
}