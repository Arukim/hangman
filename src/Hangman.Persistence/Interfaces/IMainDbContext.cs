using Hangman.Persistence.Entities;
using MongoDB.Driver;

namespace Hangman.Persistence.Interfaces
{
    public interface IMainDbContext : IDbContext
    {
        IMongoCollection<Game> Games { get; }
    }
}
