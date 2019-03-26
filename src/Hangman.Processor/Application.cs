using Hangman.Core;
using Hangman.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Hangman.Processor
{
    class Application : BaseServiceApplication<Program, ProcessorService>
    {
        protected override string Name => "Processor Service";

        protected override void BootstrapServices(IServiceCollection services)
        {
            services
                .AddSingleton<ProcessorService>()
                .AddMessaging(configuration);
        }
    }
}
