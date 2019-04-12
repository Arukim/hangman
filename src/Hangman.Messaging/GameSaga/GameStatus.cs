using System.Collections.Generic;

namespace Hangman.Messaging.GameSaga
{
    public class GameStatus : BaseSagaEvent
    {
        public string GuessedWord { get; set; }
        public List<char> Guesses { get; set; }
        public int TurnsLeft { get; set; }
        public bool HasWon { get; set; }
    }
}
