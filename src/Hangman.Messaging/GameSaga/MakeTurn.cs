namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Player's turn request
    /// </summary>
    public class MakeTurn : BaseSagaEvent
    {
        public char Guess { get; set; }
    }
}
