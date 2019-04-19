﻿using Hangman.Core;
using Hangman.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Hangman.Dictionary
{
    class DictionaryService : IService
    {
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly ILogger logger;
        private readonly IServiceProvider provider;

        private IBusControl busControl;

        public DictionaryService(ILogger<DictionaryService> logger, IOptions<RabbitMQConfiguration> rmqOptions, IServiceProvider provider)
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

                cfg.ReceiveEndpoint(host, rmqConfig.GetQueueName(Queues.Dictionary), ep =>
                {
                    ep.PrefetchCount = (ushort)Environment.ProcessorCount;

                    ep.Consumer<SelectWordConsumer>(provider);
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
