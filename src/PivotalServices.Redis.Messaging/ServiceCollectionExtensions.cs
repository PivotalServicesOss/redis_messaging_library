using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Steeltoe.CloudFoundry.Connector.Redis;

namespace PivotalServices.Redis.Messaging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisMessagingConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            AddCommonServices(services, configuration);
            services.AddSingleton<IConsumer, Consumer>();
            return services;
        }

        public static IServiceCollection AddRedisMessagingProducer(this IServiceCollection services, IConfiguration configuration)
        {
            AddCommonServices(services, configuration);
            services.AddSingleton<IProducer, Producer>();
            return services;
        }

        private static void AddCommonServices(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            if (serviceProvider.GetService<ILoggerFactory>() == null)
                services.AddLogging();

            if (serviceProvider.GetService<IConnectionMultiplexer>() == null)
                services.AddRedisConnectionMultiplexer(configuration);
        }
    }
}
