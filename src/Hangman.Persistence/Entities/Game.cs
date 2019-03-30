using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Hangman.Persistence.Entities
{
    public class Game
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
