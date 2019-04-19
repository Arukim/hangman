using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Entities;
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

        /// <summary>
        /// Process a new turn 
        /// </summary>
        public async Task Consume(ConsumeContext<ProcessTurn> ctx)
        {
            var msg = ctx.Message;

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var gameSaga = await dbContext.GameSagas
                    .Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

                // If this guess is already presented - do not accept, notify saga
                if (gameSaga.Guesses.Contains(msg.Guess))
                {
                    logger.LogInformation("Duplicate");

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

                // remove guessed letters
                gameSaga.WordLeft = gameSaga.WordLeft.Replace(msg.Guess.ToString(), "");

                bool hasGuessed = CheckGuess(gameSaga);

                gameSaga.Guesses.Add(msg.Guess);

                await dbContext.GameSagas
                    .ReplaceOneAsync(x => x.CorrelationId == gameSaga.CorrelationId, gameSaga);

                logger.LogInformation($"HasGuessed: {hasGuessed}");

                // notify saga
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

            ///<summary>
            /// Check if letter was guessed and update GuessedWord
            ///</summary>
            bool CheckGuess(GameSaga gameSaga)
            {
                var hasGuessed = false;
                for (int i = 0; i < gameSaga.Word.Length; i++)
                {
                    if (gameSaga.Word[i] == msg.Guess)
                    {
                        gameSaga.GuessedWord[i] = msg.Guess;
                        hasGuessed = true;
                    }
                }
                return hasGuessed;
            }
        }
    }
}
