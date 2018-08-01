using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Burton.Core.Domain
{
    public class BoundingBoxSizeRule : IPageRule
    {
        private const float TOLERANCE = .5f;

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            var averageBoxArea = words
                .Select(w => w.Location.Height * w.Location.Width)
                .Average();

            return words
                .Where(w =>
                    w.Location.Height * w.Location.Width > (1 - TOLERANCE) * averageBoxArea &&
                    w.Location.Height * w.Location.Width < (1 + TOLERANCE) * averageBoxArea)
                .ToList();
        }
    }
}
