using Hangman.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hangman.Dictionary.Consumers
{
    public class WordGenerator
    {
        private readonly Dictionary<Language, string[]> library = new Dictionary<Language, string[]>();
        private readonly Random random = new Random();

        /// <summary>
        /// It is hard to play with short words
        /// </summary>
        private const int minLength = 7;

        public WordGenerator()
        {
            library[Language.English] = File.ReadLines("words_eng.txt")
                    .Where(x => x.Length >= 7)
                    .ToArray();

            library[Language.Russian] = File.ReadLines("words_rus.txt")
                    .Where(x => x.Length >= 7)
                    .ToArray();
        }

        public string Get(Language lang)
        {
            var dict = library[lang];
            return dict[random.Next() % dict.Length];
        }
    }
}
