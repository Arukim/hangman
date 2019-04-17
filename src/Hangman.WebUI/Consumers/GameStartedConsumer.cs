using Hangman.Messaging.GameSaga;
using Hangman.WebUI.Controllers;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hangman.WebUI.Consumers
{
    public class GameStartedConsumer : IConsumer<GameStatus>
    {
        private readonly IHubContext<SignalRCounter> hub;
        public GameStartedConsumer(IHubContext<SignalRCounter> hub)
        {
            this.hub = hub;
        }


        public Task Consume(ConsumeContext<GameStatus> ctx)
        {
            return hub.Clients.Group(ctx.Message.CorrelationId.ToString()).SendAsync("GameState", ctx.Message);
        }
    }
}
