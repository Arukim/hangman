using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Entities;
using Hangman.Workflow;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class GameSagaTests
    {
        protected Guid correlationId;
        protected SagaTestHarness<GameSaga> sagaTestHarness;
        protected GameStateMachine stateMachine;
        protected InMemoryTestHarness harness;

        #region SetUp and TearDown
        [SetUp]
        public async Task Setup()
        {
            correlationId = Guid.NewGuid();
            stateMachine = new GameStateMachine(new Mock<ILogger<GameStateMachine>>().Object,
                Options.Create(new RabbitMQConfiguration
                {
                    Endpoint = "rabbitmq://localhost"
                }));

            harness = new InMemoryTestHarness();
            sagaTestHarness = harness.StateMachineSaga<GameSaga, GameStateMachine>(stateMachine);
            await harness.Start();
        }

        [TearDown]
        public async Task CleanUp()
        {
            await harness.Stop();
        }
        #endregion

        [Test]
        public async Task GameSaga_positive_test_case()
        {
            var testWord = "hayabusa";
            await SendAndConfirm(new Create
            {
                CorrelationId = correlationId
            });

            var saga = sagaTestHarness.Sagas.Contains(correlationId);

            Assert.AreEqual(nameof(stateMachine.Created), saga.CurrentState);

            Assert.IsTrue(harness.Sent.Select<SelectWord>().Any());

            await SendAndConfirm(new WordSelected
            {
                CorrelationId = correlationId,
                Word = testWord
            });

            Assert.AreEqual(testWord, saga.Word);
            Assert.AreEqual(10, saga.TurnsLeft);

            await SendAndConfirm(new MakeTurn
            {
                CorrelationId = correlationId
            });

            await SendAndConfirm(new TurnProcessed
            {
                CorrelationId = correlationId,
                Accepted = true,
                HasWon = false
            });
            Assert.AreEqual(9, saga.TurnsLeft);


            await SendAndConfirm(new MakeTurn
            {
                CorrelationId = correlationId
            });

            await SendAndConfirm(new TurnProcessed
            {
                CorrelationId = correlationId,
                HasWon = true
            });
        }

        public async Task SendAndConfirm<T>(T msg) where T : class
        {
            await harness.InputQueueSendEndpoint.Send(msg);
            Assert.IsTrue(harness.Consumed.Select<T>().Any());
        }
    }
}