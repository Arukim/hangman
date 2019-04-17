using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Implementation
{
    class DbContext : IDbContext
    {
        private readonly MongoDBConfiguration mongoConfig;

        public MongoClient Client { get; protected set; }
        public IMongoDatabase Database { get; protected set; }

        public IMongoCollection<GameSaga> GameSagas { get; private set; }

        public DbContext(IOptions<MongoDBConfiguration> mongoOptions)
        {
            mongoConfig = mongoOptions.Value;
            Client = new MongoClient(mongoConfig.Endpoint);
            Database = Client.GetDatabase(mongoConfig.Database);
        }

        public async Task InitAsync()
        {
            GameSagas = Database.GetCollection<GameSaga>(Collections.GameSagas);

            await GameSagas.Indexes.CreateOneAsync(new CreateIndexModel<GameSaga>(
                Builders<GameSaga>.IndexKeys.Ascending(x => x.CorrelationId), new CreateIndexOptions
                {
                    Unique = true
                }));
        }
    }
}
