namespace Hangman.Messaging
{
    /// <summary>
    /// Queues registry
    /// </summary>
    public static class Queues
    {
        public const string CreateGame = "createGame";
        public const string ProcessTurn = "processTurn";
        public const string TurnInfo = "turnInfo";
        public const string GameSaga = "gameSaga";
    }
}
