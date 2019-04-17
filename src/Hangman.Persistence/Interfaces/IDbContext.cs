using Hangman.Persistence.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Interfaces
{
    public interface IDbContext
    {
        MongoClient Client { get; }
        IMongoDatabase Database { get; }
        Task InitAsync();

        IMongoCollection<GameSaga> GameSagas { get; }
    }
}
