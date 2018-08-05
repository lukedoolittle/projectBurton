using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Domain
{
    public class Question
    {
        public QuestionType Type { get; set; }
        public int Page { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }
}
