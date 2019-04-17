using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hangman.WebUI.Controllers
{
    public class SignalRCounter : Hub
    {
        private readonly IBusControl busControl;
        private readonly RabbitMQConfiguration rmqConfig;

        public SignalRCounter(
            IBusControl busControl,
            IOptions<RabbitMQConfiguration> rmqOptions)
        {
            this.busControl = busControl;
            rmqConfig = rmqOptions.Value;
        }

        public async Task Guess(Guid id, string guess)
        {
            var ep = await busControl.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            await ep.Send(new MakeTurn { CorrelationId = id, Guess = guess.ToLowerInvariant()[0] });
        }

        public async Task Subscribe(Guid id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public async Task Unsubscribe(Guid id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
        }
    }
}
