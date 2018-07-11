using System.Collections.Generic;
using System.Linq;
using Burton.Core.Common;

namespace Burton.Core.Domain
{
    public class Page
    {
        public int PageNumber { get; set; }
        public List<WordOnPage> Words { get; set; }
        public WordOnPage ActiveWord { get; set; }

        /// <summary>
        /// Adjusts the current words and locations if the page is the same
        /// </summary>
        /// <param name="words">Words currently in view</param>
        /// <returns>True if the page is the same, false otherwise</returns>
        public bool ReconcilePage(List<WordOnPage> words)
        {
            var currentWords = Words.Select(w => w.Word);
            var newWords = Words.Select(w => w.Word);

            var firstNotSecond = currentWords.Except(newWords).ToList();
            var secondNotFirst = newWords.Except(currentWords).ToList();

            if (!firstNotSecond.Any() && !secondNotFirst.Any())
            {
                Words = words;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
