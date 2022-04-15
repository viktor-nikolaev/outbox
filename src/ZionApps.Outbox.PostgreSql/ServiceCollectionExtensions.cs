using System;
using Microsoft.Extensions.DependencyInjection;
using ZionApps.Outbox.Helpers;

namespace ZionApps.Outbox.PostgreSql
{
    public static class ServiceCollectionExtensions
    {
        public static IOutboxServicesBuilder UsePostgreSql(this IOutboxServicesBuilder services, Action<PostgreSqlOptions> configure)
        {
            services.Services.Configure(configure);
            return services.UseEventStorage<PostgreSqlEventStorage>();
        }
    }
}