using Automatonymous;
using Hangman.Core;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Hangman.Persistence.Entities
{
    public class GameSaga : SagaStateMachineInstance, IVersionedSaga
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid CorrelationId { get; set; }

        #region IVersionedSaga Implementation
        public int Version { get; set; }
        #endregion

        public string CurrentState { get; set; }
        public string Word { get; set; }
        public int TurnsLeft { get; set; }

        public char[] GuessedWord { get; set; }

        public string WordLeft { get; set; }

        public List<char> Guesses { get; set; }
        public Language Language { get; set; }
    }
}
