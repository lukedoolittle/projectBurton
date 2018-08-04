using System;
using System.Collections.Generic;
using Burton.Core.Common;

namespace Burton.Core.Domain
{
    public class LanguageDictionary
    {
        private readonly List<string> _words;
        public string Language { get; }

        public LanguageDictionary(
            List<string> words, 
            string language)
        {
            _words = words;
            Language = language;
        }

        public bool IsWordInDictionary(string word)
        {
            return _words.Contains(word);
        }

        public string GetRandomWord()
        {
            return _words[RandomNumberGenerator.RandomNumber(_words.Count)];
        }
    }
}
