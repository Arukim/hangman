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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hangman.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangmanController : Controller
    {
        private readonly IDbContext dbContext;
        private readonly IBusControl busControl;
        private readonly RabbitMQConfiguration rmqConfig;

        public HangmanController(IDbContext dbContext,
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
            var gameSaga = new GameSaga
            {
                TurnsLeft = -1,
                CorrelationId = id,
                GuessedWord = new char[] { },
                Guesses = new List<char> { }
            };
            await dbContext.GameSagas.InsertOneAsync(gameSaga);

            var ep = await busControl.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.GameSaga));

            await ep.Send(new Create { CorrelationId = gameSaga.CorrelationId });

            return new NewGame { Id = gameSaga.Id };
        }

        [HttpGet("game/{id}")]
        public async Task<GameState> GetGame(string id)
        {
            var gameId = new ObjectId(id);

            var game = await dbContext.GameSagas
                .Find(x => x.Id == gameId)
                .FirstAsync();

            return new GameState
            {
                Id = game.Id.ToString(),
                Status = game.CurrentState,
                GuessedWord = string.Join("", game.GuessedWord),
                CorrelationId = game.CorrelationId,
                Guesses = game.Guesses,
                TurnsLeft = game.TurnsLeft
            };
        }

        public class NewGame
        {
            public ObjectId Id { get; set; }
        }
    }
}