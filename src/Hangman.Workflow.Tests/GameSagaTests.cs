using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
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
        protected Guid sagaId;
        protected SagaTestHarness<GameSagaInstance> sagaTestHarness;
        protected GameStateMachine stateMachine;
        protected InMemoryTestHarness harness;

        #region SetupAndCleanUp
        [SetUp]
        public async Task Setup()
        {
            sagaId = Guid.NewGuid();
            stateMachine = new GameStateMachine(new Mock<ILogger<GameStateMachine>>().Object,
                Options.Create(new RabbitMQConfiguration
                {
                    Endpoint = "rabbitmq://localhost"
                }));

            harness = new InMemoryTestHarness();
            sagaTestHarness = harness.StateMachineSaga<GameSagaInstance, GameStateMachine>(stateMachine);
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
                CorrelationId = sagaId
            });

            var saga = sagaTestHarness.Sagas.Contains(sagaId);

            Assert.AreEqual(nameof(stateMachine.Created), saga.CurrentState);

            Assert.IsTrue(harness.Sent.Select<SetupGame>().Any());

            await SendAndConfirm(new WordSelected
            {
                CorrelationId = sagaId,
                Word = testWord
            });

            Assert.AreEqual(testWord, saga.Word);
            Assert.AreEqual(10, saga.TurnsLeft);

            await SendAndConfirm(new MakeTurn
            {
                CorrelationId = sagaId
            });

            await SendAndConfirm(new TurnProcessed
            {
                CorrelationId = sagaId,
                HasWon = false
            });
            Assert.AreEqual(9, saga.TurnsLeft);


            await SendAndConfirm(new MakeTurn
            {
                CorrelationId = sagaId
            });

            await SendAndConfirm(new TurnProcessed
            {
                CorrelationId = sagaId,
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