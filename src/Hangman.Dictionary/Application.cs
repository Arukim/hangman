using Hangman.Core;
using Hangman.Dictionary.Consumers;
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
                .AddSingleton<WordGenerator>()
                .AddMessaging(configuration)
                .AddMassTransit();
        }
    }
}
