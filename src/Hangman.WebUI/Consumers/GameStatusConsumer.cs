using Hangman.Messaging.GameSaga;
using Hangman.WebUI.Controllers;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hangman.WebUI.Consumers
{
    public class GameStatusConsumer : IConsumer<GameStatus>
    {
        private readonly IHubContext<SignalRCounter> hub;
        public GameStatusConsumer(IHubContext<SignalRCounter> hub)
        {
            this.hub = hub;
        }

        /// <summary>
        /// Publish GameStatus update to all clients connected to
        /// this game Group (CorrelationId of saga)
        /// </summary>
        public Task Consume(ConsumeContext<GameStatus> ctx) =>
            hub.Clients.Group(ctx.Message.CorrelationId.ToString())
                .SendAsync("GameState", ctx.Message);
    }
}
