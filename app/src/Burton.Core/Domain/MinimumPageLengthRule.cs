using System;
using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class MinimumPageLengthRule : IPageRule
    {
        private const float TOLERANCE = 2;

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Count > TOLERANCE ? words : new List<WordOnPage>();
        }
    }
}
