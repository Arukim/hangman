namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Response with turn results
    /// </summary>
    public class TurnProcessed : BaseSagaEvent
    {
        public bool Accepted { get; set; }
        public string CurrentWord { get; set; }
        public bool HasWon { get; set; }
    }
}
