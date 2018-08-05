using System.Collections.Generic;
using Burton.Core.Domain;

namespace Burton.Core.Data
{
    public static class Books
    {
        public static Book TheGivingTree = new Book
        {
            Pages = new List<Page>(),
            Questions = new List<Question>
            {
                new Question
                {
                    Type = QuestionType.Comprehension,
                    Page = 4,
                    QuestionText = "What did the boy pick from the tree?",
                    Answer = "Apples"
                }
            }
        };

        public static Book GoodnightMoon = new Book
        {
            Pages = new List<Page>(),
            Questions = new List<Question>
            {
                new Question
                {
                    Type = QuestionType.Comprehension,
                    Page = 3,
                    QuestionText = "",
                    Answer = ""
                }
            }
        };
    }
}
