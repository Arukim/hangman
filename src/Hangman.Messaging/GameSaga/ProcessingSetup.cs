namespace Hangman.Messaging.GameSaga
{
    public class ProcessingSetup : BaseSagaEvent
    {
        public string GuessedWord { get; set; }
    }
}
