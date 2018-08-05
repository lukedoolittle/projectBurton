using System;
using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class FinalRule : IPageRule
    {
        private const float DISTANCE_TOLERANCE = 500;
        private const float WORD_LENGTH_TOLERANCE = 2;

        public List<WordOnPage> ApplyRule(List<WordOnPage> words)
        {
            //For temporary eliminate all non-language words
            var languageWords = words
                .Where(w => w.IsDictionaryWord.HasValue && w.IsDictionaryWord.Value)
                .ToList();

            if (languageWords.Count == 0)
            {
                return languageWords;
            }

            //Eliminate words that are, location wise, too far away from the cluster
            var meanX = languageWords.Select(w => w.Location.X).Average();
            var meanY = languageWords.Select(w => w.Location.Y).Average();

            var nearWords = languageWords.Where(w =>
                Math.Abs(w.Location.X - meanX) < DISTANCE_TOLERANCE &&
                Math.Abs(w.Location.Y - meanY) < DISTANCE_TOLERANCE)
                .ToList();

            //return nothing if the sentence is too short
            return nearWords.Count > WORD_LENGTH_TOLERANCE ? 
                nearWords : 
                new List<WordOnPage>();
        }
    }
}
