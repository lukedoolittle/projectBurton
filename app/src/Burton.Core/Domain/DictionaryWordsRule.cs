using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class DictionaryWordsRule : IPageRule
    {
        private readonly List<string> _dictionaryWords;

        public DictionaryWordsRule(List<string> dictionaryWords)
        {
            _dictionaryWords = dictionaryWords;
        }

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words
                .Where(w => _dictionaryWords.Contains(w.Word))
                .ToList();
        }
    }
}
