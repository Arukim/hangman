using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hangman.Core
{
    /// <summary>
    /// To centralize settings for different console applications
    /// </summary>
    /// <typeparam name="TProg"></typeparam>
    public abstract class AbstractApplication<TProg>
        where TProg : class
    {
        protected IConfigurationRoot configuration;
        protected IServiceProvider serviceProvider;
        protected ILogger<TProg> logger;
        protected string environmentName;
        protected string hostName;

        #region Anchestors contract
        protected abstract string Name { get; }
        protected abstract void BootstrapServices(IServiceCollection services);
        protected abstract Task DoWorkloadAsync();
        #endregion

        /// <summary>
        /// Entry point for an Application
        /// </summary>
        public async Task RunAsync(string[] args)
        {
            Console.Title = Name;

            Console.WriteLine($"{Name}");

            configuration = BuildConfiguration(args);
            serviceProvider = BuildServices();

            logger.LogInformation($"Current environment is {environmentName}");
            logger.LogInformation($"Current host is {hostName}");

            await DoWorkloadAsync();
        }

        #region Internal

        /// <summary>
        /// Build up configuration for appliation
        /// </summary>
        private IConfigurationRoot BuildConfiguration(string[] args)
        {
            environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant();

            // for kubernetes we use current Pod name as host name
            hostName = Environment.GetEnvironmentVariable("HOST_NAME") ?? "default";

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // default settings
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                // support different environments using default aspnet core settings variable
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                // this configuration can be mapped into a docker container
                // in docker-compose or in kubernetes host
                .AddJsonFile(Constants.LinuxConfigPath, optional: true, reloadOnChange: true)
                // any env settings
                .AddEnvironmentVariables()
                // User settings from VS Windows UserSettings feature
                .AddUserSecrets<TProg>()
                // CLI arguments
                .AddCommandLine(args)
                .Build();
        }

        /// <summary>
        /// Build up service provider for DI engine
        /// </summary>
        private IServiceProvider BuildServices()
        {
            var serviceCollection = new ServiceCollection()
               .AddLogging((logging) =>
               {
                   logging.AddConfiguration(configuration.GetSection("Logging"));
                   logging.AddConsole();
               })
               .AddOptions();

            BootstrapServices(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();

            logger = services.GetService<ILogger<TProg>>();

            return services;
        }

        #endregion
    }
}
