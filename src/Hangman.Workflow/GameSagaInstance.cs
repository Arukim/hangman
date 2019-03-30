using Automatonymous;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hangman.Workflow
{
    public class GameSagaInstance : SagaStateMachineInstance, IVersionedSaga
    {
        public ObjectId Id { get; set; }
        [Key]
        public Guid CorrelationId { get; set; }

        #region IVersionedSaga Implementation
        public int Version { get; set; }
        #endregion

        public string CurrentState { get; set; }
        public string Word { get; set; }
        public int TurnsLeft { get; set; }
    }
}
