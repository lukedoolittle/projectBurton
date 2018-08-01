using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class PageRules
    {
        private readonly List<IPageRule> _rules = new List<IPageRule>();
        private IPageRule _finalRule;

        public PageRules AddRule(IPageRule rule)
        {
            _rules.Add(rule);

            return this;
        }

        public PageRules AddFinalRule(IPageRule rule)
        {
            _finalRule = rule;

            return this;
        }

        public List<WordOnPage> ApplyRules(List<WordOnPage> words)
        {
            var remainingWords = words;

            foreach (var rule in _rules)
            {
                remainingWords = rule.ApplyRule(remainingWords);
            }

            return _finalRule != null ? 
                _finalRule.ApplyRule(remainingWords) : 
                remainingWords;
        }

    }
}
