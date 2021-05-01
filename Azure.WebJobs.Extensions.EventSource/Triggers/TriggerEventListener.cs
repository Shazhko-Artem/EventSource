using EventSource.Client.Abstractions;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;

namespace Azure.WebJobs.Extensions.EventSource.Triggers
{
    public class TriggerEventListener : IListener
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ITriggeredFunctionExecutor contextExecutor;
        private readonly IEventSourceClient client;
        private readonly string eventName;
        private bool started;
        private bool disposed;

        public TriggerEventListener(ITriggeredFunctionExecutor contextExecutor, IEventSourceClient client, string eventName)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.contextExecutor = contextExecutor;
            this.client = client;
            this.eventName = eventName;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (this.started)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            this.client.OnReceived += this.ProcessMessageAsync;
            this.started = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (!this.started)
            {
                throw new InvalidOperationException("The listener has not yet been started or has already been stopped");
            }

            this.client.OnReceived -= this.ProcessMessageAsync;
            this.started = false;

            return Task.CompletedTask;
        }

        public void Cancel()
        {
            this.ThrowIfDisposed();
            this.cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (this.disposed) return;
            this.Cancel();
            this.disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }

        private void ProcessMessageAsync(object sender, EventMessage data)
        {
            if (data.Name != eventName)
            {
                return;
            }

            this.contextExecutor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = data }, this.cancellationTokenSource.Token);
        }
    }
}