using Hangman.Dictionary.Consumers;
using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Hangman.Dictionary
{
    public class SetupGameConsumer : IConsumer<SelectWord>
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration rmqConfig;
        private readonly WordGenerator wordGenerator;

        public SetupGameConsumer(ILogger<SetupGameConsumer> logger,
            IOptions<RabbitMQConfiguration> rmqOption,
            WordGenerator wordGenerator)
        {
            rmqConfig = rmqOption.Value;
            this.logger = logger;
            this.wordGenerator = wordGenerator;
        }

        public async Task Consume(ConsumeContext<SelectWord> ctx)
        {
            var msg = ctx.Message;

            using (var scope = logger.BeginScope($"CorrelationId={msg.CorrelationId}"))
            {

                var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

                var word = wordGenerator.Get();

                logger.LogInformation($"Selected word {word}");

                await ep.Send(new WordSelected
                {
                    CorrelationId = msg.CorrelationId,
                    Word = word
                });

                logger.LogTrace("Consumed");
            }
        }
    }
}
