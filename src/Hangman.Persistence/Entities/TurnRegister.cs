using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Hangman.Persistence.Entities
{
    public class TurnRegister
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("batchId")]
        public Guid CorrelationId { get; set; }

        [BsonElement("word")]
        public string Word { get; set; }

        [BsonElement("wordLeft")]
        public string WordLeft { get; set; }

        [BsonElement("guesses")]
        public List<char> Guesses { get; set; }
    }
}
