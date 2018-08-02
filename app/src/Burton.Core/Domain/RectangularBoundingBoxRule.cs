using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burton.Core.Domain
{
    public class RectangularBoundingBoxRule : IPageRule
    {
        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Where(w => w.Location.Width * 2 >= w.Location.Height).ToList();
        }
    }
}
