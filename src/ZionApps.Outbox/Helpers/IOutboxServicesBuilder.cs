using Microsoft.Extensions.DependencyInjection;

namespace ZionApps.Outbox.Helpers
{
    public interface IOutboxServicesBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class OutboxServicesBuilder : IOutboxServicesBuilder
    {
        public OutboxServicesBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public static class OutboxServicesBuilderExtensions
    {
        public static IOutboxServicesBuilder UseEventStorage<T>(this IOutboxServicesBuilder builder)
            where T : class, IEventStorage
        {
            builder.Services.AddScoped<IEventStorage, T>();
            return builder;
        }

        public static IOutboxServicesBuilder UseEventTransport<T>(this IOutboxServicesBuilder builder)
            where T : class, IEventTransport
        {
            builder.Services.AddScoped<IEventTransport, T>();
            return builder;
        }
    }
}