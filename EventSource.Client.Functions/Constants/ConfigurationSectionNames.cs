namespace EventSource.Client.Functions.Constants
{
    internal static class ConfigurationSectionNames
    {
        public static class EventSource
        {
            private const string Name = "EventSource";

            public const string Connection = Name + ":ConnectionString";

            public static class EventNames
            {
                public const string CreatedUser = "CreatedUser";
                public const string UpdatedUser = "UpdatedUser";
            }
        }
    }
}