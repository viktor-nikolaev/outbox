using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZionApps.Outbox.AspNetCore
{
    internal class OutboxProcessorWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OutboxProcessorWorker> _logger;

        public OutboxProcessorWorker(IServiceProvider services, ILogger<OutboxProcessorWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processing service starting");

            try
            {
                using var scope = _services.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                await outboxProcessor.StartProcessingAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred during outbox processing, stopping worker service");
                throw;
            }
        }
    }
}