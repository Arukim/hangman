using Hangman.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Persistence.Implementation
{
    abstract class BaseDbContext : IDbContext
    {
        private readonly MongoDBConfiguration mongoConfig;

        public MongoClient Client { get; protected set; }
        public IMongoDatabase Database { get; protected set; }

        public BaseDbContext(IOptions<MongoDBConfiguration> mongoOptions)
        {
            mongoConfig = mongoOptions.Value;
        }

        protected void Init()
        {
            Client = new MongoClient(mongoConfig.Endpoint);
            Database = Client.GetDatabase(mongoConfig.Database);
        }

        public abstract Task InitAsync();
    }
}
