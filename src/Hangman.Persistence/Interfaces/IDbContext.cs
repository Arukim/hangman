using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Persistence.Interfaces
{
    public interface IDbContext
    {
        MongoClient Client { get; }
        IMongoDatabase Database { get; }
        Task InitAsync();
    }
}
