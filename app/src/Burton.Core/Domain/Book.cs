using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class Book
    {
        public List<Page> Pages { get; set; }
        public List<QuestionAnswerPair> Questions { get; set; }
    }
}
