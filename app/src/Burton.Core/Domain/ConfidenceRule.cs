using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class ConfidenceRule : IPageRule
    {
        private const float TOLERANCE = .8f;
        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Where(w => w.Confidence > TOLERANCE).ToList();
        }
    }
}
