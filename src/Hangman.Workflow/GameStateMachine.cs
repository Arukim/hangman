using Automatonymous;
using Automatonymous.Binders;
using Hangman.Messaging;
using Hangman.Messaging.GameSaga;
using Hangman.Persistence.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Hangman.Workflow
{
    public class GameStateMachine : MassTransitStateMachine<GameSaga>
    {
        protected readonly ILogger<GameStateMachine> logger;
        protected readonly RabbitMQConfiguration rmqConfig;

        public GameStateMachine(ILogger<GameStateMachine> logger, IOptions<RabbitMQConfiguration> rmqOptions)
        {
            this.logger = logger;
            rmqConfig = rmqOptions.Value;

            BuildStateMachine();
        }

        #region Saga States


        /// <summary>
        /// Game is waiting for setup to finish
        /// </summary>
        public State Creating { get; set; }

        /// <summary>
        /// Game is created and waiting for setup to finish
        /// </summary>
        public State Created { get; set; }
        /// <summary>
        /// Wating for a new turn from player
        /// </summary>
        public State WaitingForTurn { get; set; }
        /// <summary>
        /// Turn is being processed 
        /// </summary>
        public State ProcessingTurn { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// A message to start a new saga receveived
        /// </summary>
        public Event<Create> CreateSagaReceived { get; set; }
        /// <summary>
        /// Quiz word for current session was selected
        /// </summary>
        public Event<WordSelected> WordSelectedReceived { get; set; }
        /// <summary>
        /// Processing has been setup
        /// </summary>
        public Event<ProcessingSetup> ProcessingSetupReceived { get; set; }
        /// <summary>
        /// Player submitted a turn
        /// </summary>
        public Event<MakeTurn> MakeTurnReceived { get; set; }
        /// <summary>
        /// Turn was processed 
        /// </summary>
        public Event<TurnProcessed> TurnProcessedRecevied { get; set; }
        /// <summary>
        /// *synthetic event*
        /// </summary>
        public Event WonGame { get; set; }
        /// <summary>
        /// *synthetic event*
        /// </summary>
        public Event LostGame { get; set; }

        #endregion

        /// <summary>
        /// Describe state machine
        /// </summary>
        private void BuildStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // Bind all events

            Event(() => CreateSagaReceived);
            Event(() => WordSelectedReceived);
            Event(() => ProcessingSetupReceived);
            Event(() => MakeTurnReceived);
            Event(() => TurnProcessedRecevied);

            // Describe all events reactions

            Initially(
                HandleCreate()
                    .TransitionTo(Creating));

            During(Creating,
                HandleWordSelected()
                    .TransitionTo(Created));

            During(Created,
                HandleProcessingSetup()
                    .TransitionTo(WaitingForTurn)
              );

            During(WaitingForTurn,
                HandleMakeTurn()
                    .TransitionTo(ProcessingTurn));

            During(ProcessingTurn,
                HandleTurnProcessed()
                    .TransitionTo(WaitingForTurn),
                HandleConcurrentTurn(),
                HandleWonGame()
                    .Finalize(),
                HandleLostGame()
                    .Finalize());
        }

        #region Handlers 

        private EventActivityBinder<GameSaga, Create> HandleCreate() =>
            When(CreateSagaReceived)
                    .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Saga created")))
                    .ThenAsync(async ctx =>
                    {
                        var saga = ctx.Instance;

                        var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.Dictionary));
                        await ep.Send(new SetupGame
                        {
                            CorrelationId = saga.CorrelationId
                        });
                    })
                    .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Setup game sent")));

        private EventActivityBinder<GameSaga, WordSelected> HandleWordSelected() =>
            When(WordSelectedReceived)
                    .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Word selected received")))
                    .ThenAsync(async ctx =>
                    {
                        var saga = ctx.Instance;

                        saga.TurnsLeft = 7;

                        var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.Processor));
                        await ep.Send(new SetupProcessing
                        {
                            CorrelationId = saga.CorrelationId,
                            Word = ctx.Data.Word
                        });
                    });

        private EventActivityBinder<GameSaga, ProcessingSetup> HandleProcessingSetup() =>
            When(ProcessingSetupReceived)
            .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Processing setup received")))
            .ThenAsync(async ctx =>
            {
                var saga = ctx.Instance;

                await ctx.Publish(new GameStatus
                {
                    Id = saga.Id.ToString(),
                    Status = saga.CurrentState,
                    CorrelationId = saga.CorrelationId,
                    Guesses = new List<char> { },
                    GuessedWord = ctx.Data.GuessedWord,
                    TurnsLeft = saga.TurnsLeft
                });
            });

        private EventActivityBinder<GameSaga, MakeTurn> HandleMakeTurn() =>
            When(MakeTurnReceived)
                    .Then(ctx => logger.LogInformation(SagaMessage(ctx, "MakeTurn received")))
                    .ThenAsync(async ctx =>
                    {
                        var saga = ctx.Instance;

                        var ep = await ctx.GetSendEndpoint(rmqConfig.GetEndpoint(Queues.Processor));
                        await ep.Send(new ProcessTurn
                        {
                            CorrelationId = saga.CorrelationId,
                            Guess = ctx.Data.Guess
                        });
                    })
                    .Then(ctx => logger.LogInformation(SagaMessage(ctx, "ProcessTurn sent")));

        /// <summary>
        /// If there is recieved a Turn request while another turn is processed
        /// by processor service - reschedule it
        /// </summary>
        private EventActivityBinder<GameSaga, MakeTurn> HandleConcurrentTurn() =>
            When(MakeTurnReceived)
                .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Concurrent MakeTurn received")))
                .ThenAsync(async ctx =>
                {
                    var consumer = ctx.CreateConsumeContext();

                    // For production-grade code we need to add some retry / backoff policy
                    // Otherwise system may be flooded when no Processors are available
                    await consumer.ScheduleSend(rmqConfig.GetEndpoint(Queues.GameSaga),
                        DateTime.UtcNow.AddMilliseconds(250),
                        ctx.Data);
                });

        private EventActivityBinder<GameSaga, TurnProcessed> HandleTurnProcessed() =>
            When(TurnProcessedRecevied)
                .Then(ctx => logger.LogInformation(SagaMessage(ctx, "TurnProcessed received")))
                .ThenAsync(async ctx =>
                {
                    var msg = ctx.Data;
                    var saga = ctx.Instance;

                    if (msg.Accepted && !msg.HasGuessed)
                    {
                        saga.TurnsLeft--;
                    }

                    await ctx.Publish(new GameStatus
                    {
                        Id = saga.Id.ToString(),
                        Status = saga.CurrentState,
                        CorrelationId = saga.CorrelationId,
                        GuessedWord = msg.GuessedWord,
                        Guesses = msg.Guesses,
                        TurnsLeft = saga.TurnsLeft,
                        HasWon = ctx.Data.HasWon
                    });

                    if (ctx.Data.HasWon)
                    {
                        await ctx.Raise(WonGame);
                        return;
                    }

                    if (saga.TurnsLeft == 0)
                    {
                        await ctx.Raise(LostGame);
                    }
                });

        private EventActivityBinder<GameSaga> HandleWonGame() =>
            When(WonGame)
                 .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Won game")));


        private EventActivityBinder<GameSaga> HandleLostGame() =>
            When(LostGame)
                 .ThenAsync(async ctx =>
                 {
                     var saga = ctx.Instance;
                     await ctx.Publish(new GameStatus
                     {
                         Id = saga.Id.ToString(),
                         Status = saga.CurrentState,
                         CorrelationId = saga.CorrelationId,
                         GuessedWord = saga.Word,
                         Guesses = saga.Guesses,
                         TurnsLeft = saga.TurnsLeft,
                         HasWon = false
                     });
                 })
                 .Then(ctx => logger.LogInformation(SagaMessage(ctx, "Lost game")));

        #endregion

        #region Aux

        protected string SagaMessage(InstanceContext<GameSaga> ctx, string msg) => $"{ctx.Instance.CorrelationId} | {msg}";

        #endregion
    }
}
