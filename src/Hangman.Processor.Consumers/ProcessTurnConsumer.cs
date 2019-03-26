using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence;
using Hangman.Persistence.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Processor.Consumers
{
    public class ProcessTurnConsumer : IConsumer<ProcessTurn>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly IProcessorDbContext dbContext;

        public ProcessTurnConsumer(ILogger<ProcessTurnConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IProcessorDbContext dbContext)
        {
            rmqConfig = rmqOption.Value;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessTurn> ctx)
        {
            var msg = ctx.Message;
            var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            await dbContext.InitAsync();

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var turnRegister = await dbContext.TurnRegisters
                    .Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                if (turnRegister.Guesses.Contains(msg.Guess))
                {
                    await ep.Send(new TurnProcessed
                    {
                        CorrelationId = msg.CorrelationId,
                        Accepted = false,
                        HasWon = false
                    });

                    return;
                }

                turnRegister.WordLeft.Replace(msg.Guess.ToString(), "");

                turnRegister.Guesses.Add(msg.Guess);

                await dbContext.TurnRegisters
                    .ReplaceOneAsync(x => x.CorrelationId == turnRegister.CorrelationId, turnRegister);

                await ep.Send(new TurnProcessed
                {
                    CorrelationId = msg.CorrelationId,
                    Accepted = false,
                    HasWon = string.IsNullOrEmpty(turnRegister.WordLeft)
                });
            }
        }
    }
}
