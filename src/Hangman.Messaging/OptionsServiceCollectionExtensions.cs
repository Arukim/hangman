using Hangman.Core;
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

    }
}
