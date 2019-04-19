using Hangman.Messaging.GameSaga;
using Hangman.WebUI.Controllers;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hangman.WebUI.Consumers
{
    public class GameStateConsumer : IConsumer<GameState>
    {
        private readonly IHubContext<SignalRCounter> hub;
        public GameStateConsumer(IHubContext<SignalRCounter> hub)
        {
            this.hub = hub;
        }

        /// <summary>
        /// Publish GameStatus update to all clients connected to
        /// this game Group (CorrelationId of saga)
        /// </summary>
        public Task Consume(ConsumeContext<GameState> ctx) =>
            hub.Clients.Group(ctx.Message.CorrelationId.ToString())
                .SendAsync("GameState", ctx.Message);
    }
}
