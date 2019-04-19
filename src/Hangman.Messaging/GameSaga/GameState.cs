using Hangman.Core;
using System.Collections.Generic;

namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Event to update WebUI state
    /// </summary>
    public class GameState : BaseSagaEvent
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string GuessedWord { get; set; }
        public List<char> Guesses { get; set; }
        public int TurnsLeft { get; set; }
        public bool HasWon { get; set; }
        public Language Language { get; set; }
    }
}
