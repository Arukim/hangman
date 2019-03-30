using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Implementation
{
    class MainDbContext : BaseDbContext, IMainDbContext
    {
        public MainDbContext(IOptions<MongoDBConfiguration> mongoOptions) : base(mongoOptions) { }

        public IMongoCollection<Game> Games { get; private set; }

        public override Task InitAsync()
        {
            Init();

            Games = Database.GetCollection<Game>("games");

            return Task.FromResult(0);
        }
    }
}
