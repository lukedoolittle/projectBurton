using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class BadCharactersRule : IPageRule
    {
        private readonly List<string> _badChars = new List<string>
        {
            "(", ")", ".", ",", "\""
        };
        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Where(w => !_badChars.Any(c => w.Word.Contains(c))).ToList();
        }
    }
}
