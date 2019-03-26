namespace Hangman.Messaging.GameSaga
{
    public class SetupProcessing : BaseSagaEvent
    {
        public string Word { get; set; }
    }
}
