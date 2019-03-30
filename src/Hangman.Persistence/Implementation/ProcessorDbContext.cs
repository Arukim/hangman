using Hangman.Persistence.Entities;
using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Implementation
{
    internal class ProcessorDbContext : BaseDbContext, IProcessorDbContext
    {
        public ProcessorDbContext(IOptions<MongoDBConfiguration> mongoOptions) : base(mongoOptions) { }

        public IMongoCollection<TurnRegister> TurnRegisters { get; private set; }

        public override async Task InitAsync()
        {
            Init();

            TurnRegisters = Database.GetCollection<TurnRegister>("turnRegisters");
            await TurnRegisters.Indexes.CreateOneAsync(new CreateIndexModel<TurnRegister>(
                Builders<TurnRegister>.IndexKeys.Ascending(x => x.CorrelationId), new CreateIndexOptions
                {
                    Unique = true
                }));
        }
    }
}
