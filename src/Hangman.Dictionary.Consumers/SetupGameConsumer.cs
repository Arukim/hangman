using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Hangman.Dictionary
{
    public class SetupGameConsumer : IConsumer<SetupGame>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;

        public SetupGameConsumer(ILogger<SetupGameConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption)
        {
            rmqConfig = rmqOption.Value;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SetupGame> ctx)
        {
            var msg = ctx.Message;

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {

                var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

                var word = "Mandragora";

                logger.LogTrace($"Selected word {word}");

                await ep.Send(new WordSelected
                {
                    CorrelationId = msg.CorrelationId,
                    Word = word
                });

                logger.LogInformation("Consumed");
            }
        }
    }
}
