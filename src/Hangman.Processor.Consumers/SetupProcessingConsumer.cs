using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence;
using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Hangman.Processor.Consumers
{
    public class SetupProcessingConsumer : IConsumer<SetupProcessing>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly IProcessorDbContext dbContext;

        public SetupProcessingConsumer(ILogger<SetupProcessingConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IProcessorDbContext dbContext)
        {
            rmqConfig = rmqOption.Value;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SetupProcessing> ctx)
        {
            var msg = ctx.Message;

            await dbContext.InitAsync();

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var turnRegister = new TurnRegister { CorrelationId = msg.CorrelationId, WordLeft = msg.Word, Word = msg.Word };

                await dbContext.TurnRegisters.InsertOneAsync(turnRegister);
            }
        }
    }
}