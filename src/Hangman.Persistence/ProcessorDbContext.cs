using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence
{
    internal class ProcessorDbContext : IProcessorDbContext
    {
        private readonly MongoDBConfiguration mongoConfig;

        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<TurnRegister> TurnRegisters { get; private set; }

        public ProcessorDbContext(IOptions<MongoDBConfiguration> mongoOptions)
        {
            mongoConfig = mongoOptions.Value;
        }

        public async Task InitAsync()
        {
            Client = new MongoClient(mongoConfig.Endpoint);
            Database = Client.GetDatabase(mongoConfig.Database);
            TurnRegisters = Database.GetCollection<TurnRegister>("turnRegisters");
            await TurnRegisters.Indexes.CreateOneAsync(new CreateIndexModel<TurnRegister>(
                Builders<TurnRegister>.IndexKeys.Ascending(x => x.CorrelationId), new CreateIndexOptions
                {
                    Unique = true
                }));
        }
    }
}
