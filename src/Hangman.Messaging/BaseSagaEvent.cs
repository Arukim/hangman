using MassTransit;
using System;

namespace Hangman.Messaging
{
    /// <summary>
    /// Basic entity for all Saga-related messages
    /// </summary>
    public class BaseSagaEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
