﻿namespace Hangman.Messaging.GameSaga
{
    /// <summary>
    /// Request to process players turn
    /// </summary>
    public class ProcessTurn : BaseSagaEvent
    {
        public char Guess { get; set; }
    }
}
