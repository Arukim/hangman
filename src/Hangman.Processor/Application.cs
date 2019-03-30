using Hangman.Core;
using Hangman.Messaging;
using Hangman.Persistence;
using Hangman.Processor.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Hangman.Processor
{
    class Application : BaseServiceApplication<Program, ProcessorService>
    {
        protected override string Name => "Processor Service";

        protected override void BootstrapServices(IServiceCollection services)
        {
            services
                .AddScoped<ProcessTurnConsumer>()
                .AddScoped<SetupProcessingConsumer>()
                .AddSingleton<ProcessorService>()
                .AddPersistence(configuration)
                .AddMessaging(configuration)
                .AddMassTransit();
        }
    }
}
