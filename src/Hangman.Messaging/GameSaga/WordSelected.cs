namespace Hangman.Messaging.GameSaga
{
    public class WordSelected : BaseSagaEvent
    {
        public string Word { get; set; }
    }
}
