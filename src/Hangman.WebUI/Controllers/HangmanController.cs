using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Hangman.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangmanController : Controller
    {
        private readonly IMainDbContext dbContext;
        private readonly IBusControl busControl;
        private readonly RabbitMQConfiguration rmqConfig;

        public HangmanController(IMainDbContext dbContext,
            IBusControl busControl,
            IOptions<RabbitMQConfiguration> rmqOptions)
        {
            this.dbContext = dbContext;
            this.busControl = busControl;
            rmqConfig = rmqOptions.Value;
        }

        [HttpPost("game")]
        public async Task<NewGame> PostGame()
        {
            var id = Guid.NewGuid();
            var game = new Game { CorrelationId = id };
            await dbContext.Games.InsertOneAsync(game);

            var ep = await busControl.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            await ep.Send(new Create { CorrelationId = game.CorrelationId });

            return new NewGame { Id = game.Id };
        }

        [HttpGet("game/{id}")]
        public async Task<GameInfo> GetGame(ObjectId id)
        {
            var game = await dbContext.Games
                .Find(x => x.Id == id)
                .FirstAsync();

            return new GameInfo
            {
                Id = game.Id
            };
        }

        public class NewGame
        {
            public ObjectId Id { get; set; }
        }

        public class GameInfo
        {
            public ObjectId Id { get; set; }
        }
    }
}