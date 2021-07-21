using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Models;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventSource.Client.Functions.Functions
{
    public class HandleEventFunction
    {
        private readonly IStore<EventMessage> store;

        public HandleEventFunction(IStore<EventMessage> store)
        {
            this.store = store;
        }

        [FunctionName(nameof(HandleCreatedUserEventFunction))]
        public void HandleCreatedUserEventFunction(
            [EventSourceTrigger(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, ConfigurationSectionNames.EventSource.Connection)] string message, ILogger log)
        {
            log.LogInformation($"C# EventSource trigger function processed message: {message}");
            this.store.Add(new StringEventMessage(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, message));
        }

        [FunctionName(nameof(HandleUpdatedUserEventFunction))]
        public void HandleUpdatedUserEventFunction(
            [EventSourceTrigger(ConfigurationSectionNames.EventSource.EventNames.UpdatedUser, ConfigurationSectionNames.EventSource.Connection)] User message, ILogger log)
        {
            log.LogInformation($"C#  EventSource trigger function processed message: {message.UserName}");
            this.store.Add(new CustomEventMessage<User>(ConfigurationSectionNames.EventSource.EventNames.UpdatedUser, message));
        }

        [FunctionName(nameof(HandleDeletedUserEventFunction))]
        public void HandleDeletedUserEventFunction(
            [EventSourceTrigger("DeletedUser", ConfigurationSectionNames.EventSource.Connection)] CustomEventMessage<User> message, ILogger log)
        {
            log.LogInformation($"C#  EventSource trigger function processed message: {message.Content.UserName}");
            this.store.Add(message);
        }

        [FunctionName(nameof(HandleCreatingUserErrorEventFunction))]
        public void HandleCreatingUserErrorEventFunction(
            [EventSourceTrigger("CreatingUserError", ConfigurationSectionNames.EventSource.Connection)] EventMessage message, ILogger log)
        {
            log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
            this.store.Add(message);
        }

        [FunctionName(nameof(HandleUpdatingUserErrorEventFunction))]
        public void HandleUpdatingUserErrorEventFunction(
            [EventSourceTrigger("UpdatingUserError", ConfigurationSectionNames.EventSource.Connection)] StringEventMessage message, ILogger log)
        {
            log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
            this.store.Add(message);
        }

        [FunctionName(nameof(HandleDeletingUserErrorEventFunction))]
        public void HandleDeletingUserErrorEventFunction(
            [EventSourceTrigger("DeletingUserError", ConfigurationSectionNames.EventSource.Connection)] BytesEventMessage message, ILogger log)
        {
            log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
            this.store.Add(message);
        }
    }
}