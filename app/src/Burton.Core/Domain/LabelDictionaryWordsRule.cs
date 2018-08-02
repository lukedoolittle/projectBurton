using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Domain
{
    public class LabelDictionaryWordsRule : IPageRule
    {
        private readonly List<string> _dictionaryWords;

        public LabelDictionaryWordsRule(List<string> dictionaryWords)
        {
            _dictionaryWords = dictionaryWords;
        }

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            foreach (var word in words)
            {
                word.IsDictionaryWord = _dictionaryWords.Contains(word.Word.TrimEnd('.'));
            }

            return words;
        }
    }
}
