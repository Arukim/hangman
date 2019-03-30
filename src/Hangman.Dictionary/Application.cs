using Hangman.Core;
using Hangman.Messaging;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Hangman.Dictionary
{
    class Application : BaseServiceApplication<Program, DictionaryService>
    {
        protected override string Name => "Dictionary Service";

        protected override void BootstrapServices(IServiceCollection services)
        {
            services
                .AddScoped<SetupGameConsumer>()
                .AddSingleton<DictionaryService>()
                .AddMessaging(configuration)
                .AddMassTransit();
        }
    }
}
