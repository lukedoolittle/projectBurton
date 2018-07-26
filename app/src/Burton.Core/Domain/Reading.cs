using System;
using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class Reading
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public Book Book { get; set; }

        /// <summary>
        /// Track current history of performed words
        /// </summary>
        public List<SpeechPerformance> SpeechPerformances { get; set; }

        /// <summary>
        /// Track current history of prompted questions (vocabulary or comprehension)
        /// </summary>
        public List<QuestionPerformance> QuestionPerformances { get; set; }
    }
}
