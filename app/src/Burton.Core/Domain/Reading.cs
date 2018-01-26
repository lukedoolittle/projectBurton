using System;
using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class Reading
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public Book Book { get; set; }

        public List<Assessment<string>> Phonics { get; set; }
        public List<Assessment<QuestionAnswerPair>> Comprehension { get; set; }
    }
}
