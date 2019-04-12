using System;
using System.IO;

namespace Hangman.Dictionary.Consumers
{
    public class WordGenerator
    {
        private readonly string[] words;
        private readonly Random random = new Random();

        public WordGenerator()
        {
            words = File.ReadAllLines("words_alpha.txt");
        }

        public string Get() => words[random.Next() % words.Length];
    }
}
