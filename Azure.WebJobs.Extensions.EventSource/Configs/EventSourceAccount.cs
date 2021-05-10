using System;
using System.Globalization;
using EventSource.Common.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;

namespace Azure.WebJobs.Extensions.EventSource.Configs
{
    public class EventSourceAccount
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly IConfiguration configuration;
        private readonly EventSourceConnectionOptions options;
        private string connectionString;

        public EventSourceAccount(EventSourceConnectionOptions options, IConfiguration configuration, IConnectionProvider connectionProvider)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.configuration = configuration;
            this.connectionProvider = connectionProvider;
        }

        public virtual string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = this.options.ConnectionString;
                    if (connectionProvider != null && !string.IsNullOrEmpty(connectionProvider.Connection))
                    {
                        connectionString = configuration.GetWebJobsConnectionString(connectionProvider.Connection);
                    }

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException(
                            string.Format(CultureInfo.InvariantCulture, "Microsoft Azure WebJobs SDK EventSource connection string '{0}' is missing or empty.",
                                Sanitizer.Sanitize(this.connectionProvider.Connection) ?? Constants.DefaultConnectionSettingStringName));
                    }
                }

                return connectionString;
            }
        }
    }
}