using Azure.WebJobs.Extensions.EventSource.Attributes;
using EventSource.Client.Functions.Constants;
using EventSource.Client.Functions.Services;
using EventSource.Common.Models.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventSource.Client.Functions.Functions
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }

    public class HandleEventFunction
    {
        private readonly IStore<EventMessage> store;

        public HandleEventFunction(IStore<EventMessage> store)
        {
            this.store = store;
        }

        [FunctionName("HandleCreatedUserEventFunction")]
        public void HandleCreatedUserEventFunction(
            [EventSourceTrigger(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, ConfigurationSectionNames.EventSource.Connection)] string message, ILogger log)
        {
            log.LogInformation($"C# EventSource trigger function processed message: {message}");
            this.store.Add(new StringEventMessage(ConfigurationSectionNames.EventSource.EventNames.CreatedUser, message));
        }

        //[FunctionName("HandleUpdatedUserEventFunction")]
        //public void HandleUpdatedUserEventFunction(
        //    [EventSourceTrigger(ConfigurationSectionNames.EventSource.EventNames.UpdatedUser, ConfigurationSectionNames.EventSource.Connection)] User message, ILogger log)
        //{
        //    log.LogInformation($"C#  EventSource trigger function processed message: {message.UserName}");
        //    this.store.Add(new CustomEventMessage<User>(ConfigurationSectionNames.EventSource.EventNames.UpdatedUser, message));
        //}

        //[FunctionName("HandleDeletedUserEventFunction")]
        //public void HandleDeletedUserEventFunction(
        //    [EventSourceTrigger("DeletedUser", ConfigurationSectionNames.EventSource.Connection)] CustomEventMessage<User> message, ILogger log)
        //{
        //    log.LogInformation($"C#  EventSource trigger function processed message: {message.Content.UserName}");
        //    this.store.Add(message);
        //}

        //[FunctionName("HandleCreatingUserErrorEventFunction")]
        //public void HandleCreatingUserErrorEventFunction(
        //    [EventSourceTrigger("CreatingUserError", ConfigurationSectionNames.EventSource.Connection)] EventMessage message, ILogger log)
        //{
        //    log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
        //    this.store.Add(message);
        //}

        //[FunctionName("HandleUpdatingUserErrorEventFunction")]
        //public void HandleUpdatingUserErrorEventFunction(
        //    [EventSourceTrigger("UpdatingUserError", ConfigurationSectionNames.EventSource.Connection)] StringEventMessage message, ILogger log)
        //{
        //    log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
        //    this.store.Add(message);
        //}

        //[FunctionName("HandleDeletingUserErrorEventFunction")]
        //public void HandleDeletingUserErrorEventFunction(
        //    [EventSourceTrigger("DeletingUserError", ConfigurationSectionNames.EventSource.Connection)] BytesEventMessage message, ILogger log)
        //{
        //    log.LogInformation($"C#  EventSource trigger function processed message: {message.Name}");
        //    this.store.Add(message);
        //}
    }
}