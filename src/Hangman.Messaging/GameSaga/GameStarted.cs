namespace Hangman.Messaging.GameSaga
{
    public class GameStarted : BaseSagaEvent
    {
        public int WordLength { get; set; }
        public int TotalTurns { get; set; }
    }
}
