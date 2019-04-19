using System;
using System.IO;
using System.Linq;

namespace Hangman.Dictionary.Consumers
{
    public class WordGenerator
    {
        private readonly string[] words;
        private readonly Random random = new Random();

        public WordGenerator()
        {
            words = File.ReadLines("words_alpha.txt")
                    .Where(x => x.Length >= 7)
                    .ToArray();
        }

        public string Get() => words[random.Next() % words.Length];
    }
}
