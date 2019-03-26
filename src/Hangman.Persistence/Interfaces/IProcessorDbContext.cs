using Hangman.Persistence.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Interfaces
{
    /// <summary>
    /// TODO - add abstraction layer
    /// </summary>
    public interface IProcessorDbContext
    {
        MongoClient Client { get; }
        IMongoDatabase Database { get; }
        IMongoCollection<TurnRegister> TurnRegisters { get; }
        Task InitAsync();
    }
}
