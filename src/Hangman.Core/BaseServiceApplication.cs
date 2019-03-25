using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hangman.Core
{
    /// <summary>
    /// Parent for all services application
    /// </summary>
    /// <typeparam name="TProg">Program instance</typeparam>
    /// <typeparam name="TService"></typeparam>
    public abstract class BaseServiceApplication<TProg, TService> : AbstractApplication<TProg>
       where TProg : class
       where TService : IService
    {
        // AutoResetEvent to signal when to exit the application.
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        protected override async Task DoWorkloadAsync()
        {
            // Handle Control+C or Control+Break (before StartAsync)
            // This is crucial for Docker app, since docker is spamming keypress events in CIN
            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;
                waitHandle.Set();
            };

            logger.LogInformation("Application is starting");

            var app = serviceProvider.GetService<TService>();
            await app.StartAsync();

            logger.LogInformation("Application is ready");

            Console.WriteLine("Press CTRL+C to exit application");
            // Wait for cancellation
            waitHandle.WaitOne();

            logger.LogInformation("Application is stopping");
            await app.StopAsync();
            logger.LogInformation("Application is exiting");
        }
    }
}
