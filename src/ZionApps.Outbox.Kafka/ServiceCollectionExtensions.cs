using System;
using Microsoft.Extensions.DependencyInjection;
using ZionApps.Outbox.Helpers;

namespace ZionApps.Outbox.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static IOutboxServicesBuilder UseKafka(this IOutboxServicesBuilder services, Action<KafkaOptions> configure)
        {
            services.Services.Configure(configure);
            return services.UseEventTransport<KafkaTransport>();
        }
    }
}