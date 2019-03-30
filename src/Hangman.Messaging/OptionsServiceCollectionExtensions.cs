using Hangman.Core;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hangman.Messaging
{
    public static class OptionsServiceCollectionExtensions
    {

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rmqSection = configuration.GetSection(Constants.ConfigSections.Rabbit);
            if (!rmqSection.Exists())
                throw new ApplicationException($"Required section '{rmqSection.Path}' not found in configuration");

            services.Configure<RabbitMQConfiguration>(rmqSection);

            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rmqSection = configuration.GetSection(Constants.ConfigSections.Rabbit);
            var rmqConfig = rmqSection.Get<RabbitMQConfiguration>();

            services
             .Configure<RabbitMQConfiguration>(configuration.GetSection(Constants.ConfigSections.Rabbit))
             .AddSingleton(Bus.Factory.CreateUsingRabbitMq(cfg =>
             {
                 cfg.Host(new Uri(rmqConfig.Endpoint), host =>
                 {
                     host.Username(rmqConfig.Username);
                     host.Password(rmqConfig.Password);
                 });
             }));

            return services;
        }

    }
}
