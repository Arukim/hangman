using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.Processor.Consumers
{
    public class SetupProcessingConsumer : IConsumer<SetupProcessing>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly IProcessorDbContext procCtx;
        private readonly IMainDbContext mainCtx;

        public SetupProcessingConsumer(ILogger<SetupProcessingConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            IProcessorDbContext procCtx,
            IMainDbContext mainCtx)
        {
            rmqConfig = rmqOption.Value;
            this.procCtx = procCtx;
            this.mainCtx = mainCtx;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SetupProcessing> ctx)
        {
            var msg = ctx.Message;

            await procCtx.InitAsync();

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {
                var turnRegister = new TurnRegister { CorrelationId = msg.CorrelationId, WordLeft = msg.Word, Word = msg.Word };

                await procCtx.TurnRegisters.InsertOneAsync(turnRegister);

                var game = await mainCtx.Games.Find(x => x.CorrelationId == msg.CorrelationId)
                    .FirstAsync();

                game.Word = msg.Word;

                game.GuessedWord = Enumerable.Range(0, msg.Word.Length).Select(x => '-').ToArray();

                await mainCtx.Games.ReplaceOneAsync(x => x.CorrelationId == game.CorrelationId, game);
            }
        }
    }
}