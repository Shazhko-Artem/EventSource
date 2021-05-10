using EventSource.Client.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Azure.WebJobs.Extensions.EventSource.Configs;
using Azure.WebJobs.Extensions.EventSource.Services.Connection;
using EventSource.Client.Abstractions;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using EventSource.Common.Options;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace EventSource.Client.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();

            var services = builder.Services;

            services.Configure<EventSourceConnectionOptions>(configuration.GetSection("EventSource"));
            services.AddSingleton<IEventSourceClient>(provider =>
            {
                var clientProvider = provider.GetRequiredService<IEventSourceClientProvider>();
                var options = provider.GetService<IOptions<EventSourceConnectionOptions>>().Value;
                var account = new EventSourceAccount(options, configuration: null, connectionProvider: null);
                return clientProvider.GetClient(account);
            });

            services.AddSingleton<IScopeInvestigator, ScopeInvestigator>();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(typeof(IStore<>), typeof(Store<>));
        }
    }
}