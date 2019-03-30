﻿using Hangman.Core;
using Hangman.Persistence.Implementation;
using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Hangman.Persistence
{
    public static class OptionsServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoSection = configuration.GetSection(Constants.ConfigSections.Mongo);
            if (!mongoSection.Exists())
                throw new ApplicationException($"Required section '{mongoSection.Path}' not found in configuration");

            services.Configure<MongoDBConfiguration>(mongoSection);

            var mongoDbConfig = new MongoDBConfiguration();
            mongoSection.Bind(mongoDbConfig);

            var mainDbContext = new MainDbContext(Options.Create(mongoDbConfig));
            mainDbContext.InitAsync().GetAwaiter().GetResult();

            services.AddSingleton<IMainDbContext>(mainDbContext);

            var processorDb = new ProcessorDbContext(Options.Create(mongoDbConfig));
            processorDb.InitAsync().GetAwaiter().GetResult();

            services.AddSingleton<IProcessorDbContext>(processorDb);

            return services;
        }
    }
}
