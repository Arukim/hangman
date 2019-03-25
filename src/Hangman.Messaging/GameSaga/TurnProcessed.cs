namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Response with turn results
    /// </summary>
    public class TurnProcessed : BaseSagaEvent
    {
        public bool HasWon { get; set; }
    }
}
