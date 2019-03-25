using Hangman.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hangman.Orchestrator
{
    class Application : BaseServiceApplication<Program, OrchestratorService>
    {
        protected override string Name => "Orchestrator";

        protected override void BootstrapServices(IServiceCollection services)
        {
            var mongoSection = configuration.GetSection(Constants.ConfigSections.Mongo);
            if (!mongoSection.Exists())
                throw new ApplicationException($"Required section '{mongoSection.Path}' not found in configuration");

            var rmqSection = configuration.GetSection(Constants.ConfigSections.Rabbit);
            if (!rmqSection.Exists())
                throw new ApplicationException($"Required section '{rmqSection.Path}' not found in configuration");
        }
    }
}
