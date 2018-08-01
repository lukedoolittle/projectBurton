using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burton.Core.Domain
{
    public class SingleCharactersRule : IPageRule
    {
        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Where(w => (w.Word.Length > 1) || (w.Word == "a") || (w.Word == "i")).ToList();
        }
    }
}
