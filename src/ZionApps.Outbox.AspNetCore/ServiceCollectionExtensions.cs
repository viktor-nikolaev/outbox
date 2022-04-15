using System;
using Microsoft.Extensions.DependencyInjection;
using ZionApps.Outbox.Helpers;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IOutboxServicesBuilder AddOutbox(this IServiceCollection services, Action<OutboxOptions>? configure = null)
        {
            services.AddHostedService<OutboxProcessorWorker>();
            return services.AddOutboxCore(configure);
        }
    }
}