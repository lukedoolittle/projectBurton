using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class ExtremeSizeBoundingBoxRule : IPageRule
    {
        private const float MAX_HEIGHT = 200;
        private const float MAX_WIDTH = 500;
        private const float MIN_HEIGHT = 5;
        private const float MIN_WIDTH = 5;

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            return words.Where(w =>
                w.Location.Width > MIN_WIDTH && 
                w.Location.Width < MAX_WIDTH && 
                w.Location.Height > MIN_HEIGHT &&
                w.Location.Height < MAX_HEIGHT).ToList();
        }
    }
}
