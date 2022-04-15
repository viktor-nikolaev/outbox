using System;
using Microsoft.Extensions.DependencyInjection;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IOutboxServicesBuilder AddOutboxCore(this IServiceCollection services,
            Action<OutboxOptions>? configure)
        {
            services.Configure(configure ?? (_ => { }));
            services.AddScoped<IOutbox, Outbox>();
            services.AddScoped<IOutboxProcessor, OutboxProcessor>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();

            return new OutboxServicesBuilder(services);
        }
    }
}