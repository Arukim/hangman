using Hangman.Core;
using Hangman.Messaging;
using Hangman.Persistence;
using Hangman.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace Hangman.Orchestrator
{
    class Application : BaseServiceApplication<Program, OrchestratorService>
    {
        protected override string Name => "Orchestrator Service";

        protected override void BootstrapServices(IServiceCollection services)
        {
            services
                .AddSingleton<GameStateMachine>()
                .AddSingleton<OrchestratorService>()
                .AddMessaging(configuration)
                .AddPersistence(configuration);
        }
    }
}
