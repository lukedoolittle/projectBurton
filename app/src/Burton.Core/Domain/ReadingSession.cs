using System;
using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class ReadingSession
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// The book that is part of the reading session
        /// </summary>
        public Book Book { get; set; } = new Book();

        /// <summary>
        /// Track current history of performed words
        /// </summary>
        public List<SpeechPerformance> SpeechPerformances { get; set; } = 
            new List<SpeechPerformance>();

        /// <summary>
        /// Track current history of prompted questions (vocabulary or comprehension)
        /// </summary>
        public List<QuestionPerformance> QuestionPerformances { get; set; } = 
            new List<QuestionPerformance>();
    }
}
