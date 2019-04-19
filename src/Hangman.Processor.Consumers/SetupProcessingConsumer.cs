using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Processor.Consumers
{
    public class SetupProcessingConsumer : IConsumer<SetupProcessing>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly IDbContext dbContext;

        public SetupProcessingConsumer(ILogger<SetupProcessingConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IDbContext dbContext)
        {
            rmqConfig = rmqOption.Value;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Prepare for game
        /// - Guesses
        /// - GuessedWord
        /// - WordLeft
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<SetupProcessing> ctx)
        {
            var msg = ctx.Message;

            var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var word = msg.Word.ToLowerInvariant();

                var game = await dbContext.GameSagas.Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                game.Word = word;
                game.WordLeft = word;

                game.GuessedWord = Enumerable.Range(0, word.Length)
                                            .Select(x => '-')
                                            .ToArray();

                game.Guesses = new List<char>();

                await dbContext.GameSagas.ReplaceOneAsync(x => x.CorrelationId == game.CorrelationId, game);

                logger.LogInformation("Processing setup done");

                await ep.Send(new ProcessingSetup
                {
                    CorrelationId = msg.CorrelationId,
                    GuessedWord = string.Join("", game.GuessedWord)
                });
            }
        }
    }
}