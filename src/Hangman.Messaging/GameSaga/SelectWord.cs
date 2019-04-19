using Hangman.Core;

namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Request to Setup a game
    /// </summary>
    public class SelectWord : BaseSagaEvent
    {
        public Language Language { get; set; }
    }
}
