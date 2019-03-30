using Hangman.Core;
using Hangman.Messaging;
using Hangman.Processor.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Hangman.Processor
{
    class ProcessorService : IService
    {
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly ILogger logger;
        private readonly IServiceProvider provider;

        private IBusControl busControl;

        public ProcessorService(ILogger<ProcessorService> logger, IOptions<RabbitMQConfiguration> rmqOptions, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
            rmqConfig = rmqOptions.Value;
        }

        public async Task StartAsync()
        {
            busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(rmqConfig.Endpoint), h =>
                {
                    h.Username(rmqConfig.Username);
                    h.Password(rmqConfig.Password);
                });

                cfg.ReceiveEndpoint(host, rmqConfig.GetQueueName(Queues.Processor), ep =>
                {
                    ep.PrefetchCount = (ushort)Environment.ProcessorCount;

                    ep.Consumer<ProcessTurnConsumer>(provider);
                    ep.Consumer<SetupProcessingConsumer>(provider);
                });
            });

            await busControl.StartAsync();
        }

        public async Task StopAsync()
        {
            await busControl.StopAsync();
        }
    }
}
