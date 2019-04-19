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
        #region Saga
        /// <summary>
        /// Current Saga state, shouldn't be modified by user code
        /// </summary>
        public string CurrentState { get; set; }
        /// <summary>
        /// Turns left to play, managed by Saga
        /// </summary>
        public int TurnsLeft { get; set; }
        #endregion

        /// <summary>
        /// Current word to guess
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// Current guessed word
        /// </summary>
        public char[] GuessedWord { get; set; }
        /// <summary>
        /// Letters left to guess
        /// </summary>
        public string WordLeft { get; set; }
        /// <summary>
        /// Guesses (Letters)
        /// </summary>
        public List<char> Guesses { get; set; }
        /// <summary>
        /// Game Language
        /// </summary>
        public Language Language { get; set; }
    }
}
