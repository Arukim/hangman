using Hangman.Core;
using Hangman.Messaging;
using Hangman.Persistence;
using Hangman.Persistence.Entities;
using Hangman.Workflow;
using MassTransit;
using MassTransit.MongoDbIntegration.Saga;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Hangman.Orchestrator
{
    internal class OrchestratorService : IService
    {
        private readonly MongoDBConfiguration mongoDbConfig;
        private readonly RabbitMQConfiguration rabbitMqConfig;
        private readonly ILogger logger;
        private readonly GameStateMachine stateMachine;


        private IBusControl busControl;

        public OrchestratorService(ILogger<OrchestratorService> logger,
            IOptions<MongoDBConfiguration> mongoDbOptions,
            IOptions<RabbitMQConfiguration> rabbitMqOptions,
            GameStateMachine stateMachine)
        {
            mongoDbConfig = mongoDbOptions.Value;
            rabbitMqConfig = rabbitMqOptions.Value;
            this.logger = logger;
            this.stateMachine = stateMachine;
        }

        public async Task StartAsync()
        {
            var repository = new MongoDbSagaRepository<GameSaga>(mongoDbConfig.Endpoint, mongoDbConfig.Database, Collections.GameSagas);

            busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var rabbitMqHost = cfg.Host(new Uri(rabbitMqConfig.Endpoint), host =>
                {
                    host.Username(rabbitMqConfig.Username);
                    host.Password(rabbitMqConfig.Password);
                });

                cfg.ReceiveEndpoint(rabbitMqHost, rabbitMqConfig.GetQueueName(Queues.GameSaga), e =>
                {
                    // Specify the maximum number of concurrent messages that are consumed
                    e.PrefetchCount = (ushort)Environment.ProcessorCount;
                    // Defer publish of messages until saga is persisted
                    // Required for PrefetchCount > 1 
                    // https://stackoverflow.com/questions/38716143/masstransit-saga-running-with-prefetch-1
                    e.UseInMemoryOutbox();
                    e.StateMachineSaga(stateMachine, repository);

                });

                // In-memory quartz do not provide persistence
                // For production use separate quartz service or rabbitmq scheduling plugin
                cfg.UseInMemoryScheduler();
            });

            logger.LogInformation("Starting MT bus");
            await busControl.StartAsync();
            logger.LogInformation("Started MT bus");
        }

        public async Task StopAsync()
        {

            if (busControl == null)
            {
                throw new InvalidOperationException($"The {nameof(busControl)} hasn't started yet.");
            }

            logger.LogInformation("Stopping MT bus");
            await busControl.StopAsync();
            logger.LogInformation("Stopped MT bus"); ;
        }
    }
}