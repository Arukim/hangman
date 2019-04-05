using Hangman.Messaging.GameSaga;
using Hangman.WebUI.Controllers;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hangman.WebUI.Consumers
{
    public class GameStartedConsumer : IConsumer<GameStarted>
    {
        private readonly IHubContext<SignalRCounter> hub;
        public GameStartedConsumer(IHubContext<SignalRCounter> hub)
        {
            this.hub = hub;
        }


        public Task Consume(ConsumeContext<GameStarted> ctx)
        {
            return hub.Clients.All.SendAsync("GameStarted", ctx.Message);
        }
    }
}
