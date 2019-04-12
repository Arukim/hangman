using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
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
        private readonly IProcessorDbContext processorCtx;
        private readonly IMainDbContext mainCtx;

        public ProcessTurnConsumer(ILogger<ProcessTurnConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IProcessorDbContext processorCtx,
            IMainDbContext mainCtx)
        {
            rmqConfig = rmqOption.Value;
            this.processorCtx = processorCtx;
            this.mainCtx = mainCtx;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessTurn> ctx)
        {
            var msg = ctx.Message;
            var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            await processorCtx.InitAsync();

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var turnRegister = await processorCtx.TurnRegisters
                    .Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                var game = await mainCtx.Games.Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                if (turnRegister.Guesses.Contains(msg.Guess))
                {
                    await ep.Send(new TurnProcessed
                    {
                        CorrelationId = msg.CorrelationId,
                        Accepted = false,
                        GuessedWord = string.Join("", game.GuessedWord),
                        Guesses = turnRegister.Guesses,
                        HasWon = false
                    });

                    return;
                }

                turnRegister.WordLeft = turnRegister.WordLeft.Replace(msg.Guess.ToString(), "");

                bool hasGuessed = false;
                for (int i = 0; i < turnRegister.Word.Length; i++)
                {
                    if (turnRegister.Word[i] == msg.Guess)
                    {
                        game.GuessedWord[i] = msg.Guess;
                        hasGuessed = true;
                    }
                }

                turnRegister.Guesses.Add(msg.Guess);

                await processorCtx.TurnRegisters
                    .ReplaceOneAsync(x => x.CorrelationId == turnRegister.CorrelationId, turnRegister);

                await mainCtx.Games.ReplaceOneAsync(x => x.CorrelationId == game.CorrelationId, game);

                await ep.Send(new TurnProcessed
                {
                    CorrelationId = msg.CorrelationId,
                    Accepted = true,
                    GuessedWord = string.Join("", game.GuessedWord),
                    Guesses = turnRegister.Guesses,
                    HasWon = string.IsNullOrEmpty(turnRegister.WordLeft),
                    HasGuessed = hasGuessed
                });
            }
        }
    }
}
