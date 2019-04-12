using System.Collections.Generic;

namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Response with turn results
    /// </summary>
    public class TurnProcessed : BaseSagaEvent
    {
        public bool Accepted { get; set; }
        public string GuessedWord { get; set; }
        public List<char> Guesses { get; set; }
        public bool HasWon { get; set; }
        public bool HasGuessed { get; set; }
    }
}
