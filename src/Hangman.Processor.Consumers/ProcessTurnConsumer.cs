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
        private readonly IDbContext dbContext;

        public ProcessTurnConsumer(ILogger<ProcessTurnConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IDbContext dbContext)
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
                var gameSaga = await dbContext.GameSagas
                    .Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                if (gameSaga.Guesses.Contains(msg.Guess))
                {
                    await ep.Send(new TurnProcessed
                    {
                        CorrelationId = msg.CorrelationId,
                        Accepted = false,
                        GuessedWord = string.Join("", gameSaga.GuessedWord),
                        Guesses = gameSaga.Guesses,
                        HasWon = false
                    });

                    return;
                }

                gameSaga.WordLeft = gameSaga.WordLeft.Replace(msg.Guess.ToString(), "");

                bool hasGuessed = false;
                for (int i = 0; i < gameSaga.Word.Length; i++)
                {
                    if (gameSaga.Word[i] == msg.Guess)
                    {
                        gameSaga.GuessedWord[i] = msg.Guess;
                        hasGuessed = true;
                    }
                }

                gameSaga.Guesses.Add(msg.Guess);

                await dbContext.GameSagas
                    .ReplaceOneAsync(x => x.CorrelationId == gameSaga.CorrelationId, gameSaga);

                await ep.Send(new TurnProcessed
                {
                    CorrelationId = msg.CorrelationId,
                    Accepted = true,
                    GuessedWord = string.Join("", gameSaga.GuessedWord),
                    Guesses = gameSaga.Guesses,
                    HasWon = string.IsNullOrEmpty(gameSaga.WordLeft),
                    HasGuessed = hasGuessed
                });
            }
        }
    }
}
