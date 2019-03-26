using Hangman.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hangman.Persistence
{
    public static class OptionsServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var mongoSection = configuration.GetSection(Constants.ConfigSections.Mongo);
            if (!mongoSection.Exists())
                throw new ApplicationException($"Required section '{mongoSection.Path}' not found in configuration");

            services.Configure<MongoDBConfiguration>(mongoSection);

            services.AddScoped<ProcessorDbContext>();


            return services;
        }
    }
}
