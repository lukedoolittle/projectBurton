using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Domain
{
    public interface IPageRule
    {
        List<WordOnPage> ApplyRule(List<WordOnPage> words);
    }
}
