using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class LabelDictionaryWordsRule : IPageRule
    {
        private readonly LanguageDictionary _dictionaryWords;

        public LabelDictionaryWordsRule(LanguageDictionary dictionaryWords)
        {
            _dictionaryWords = dictionaryWords;
        }

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            foreach (var word in words)
            {
                word.IsDictionaryWord = _dictionaryWords.IsWordInDictionary(word.Word.TrimEnd('.'));
            }

            return words;
        }
    }
}
