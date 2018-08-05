using System.Collections.Generic;
using System.Linq;
using Burton.Core.Domain;

namespace Burton.Core.Data
{
    public static class Books
    {
        private static List<WordOnPage> ParseText(string text)
        {
            return text.Split(' ').Select(t => new WordOnPage {Word = t}).ToList();
        }

        public static Book TheGivingTree = new Book
        {
            Pages = new List<Page>
            {
                new Page
                {
                    PageNumber = 1,
                    Words = ParseText("once there was a tree")
                },
                new Page
                {
                    PageNumber = 2,
                    Words = ParseText("and she loved a little boy")
                },
                new Page
                {
                    PageNumber = 3,
                    Words = ParseText("and every day the boy would come")
                },
                new Page
                {
                    PageNumber = 4,
                    Words = ParseText("and he would gather her leaves")
                },
                new Page
                {
                    PageNumber = 5,
                    Words = ParseText("and make them into crowns and play king of the forest")
                },
                new Page
                {
                    PageNumber = 6,
                    Words = ParseText("he would climb up her trunk")
                },
                new Page
                {
                    PageNumber = 7,
                    Words = ParseText("and swing from her branches")
                },
                new Page
                {
                    PageNumber = 8,
                    Words = ParseText("and eat apples")
                }
            },
            Questions = new List<Question>
            {
                //new Question
                //{
                //    Type = QuestionType.Comprehension,
                //    Page = 1,
                //    QuestionText = "Who is in the story?",
                //    Answer = "tree"
                //},
                new Question
                {
                    Type = QuestionType.Comprehension,
                    Page = 4,
                    QuestionText = "What did the boy gather?",
                    Answer = "leaves"
                },
                new Question
                {
                    Type = QuestionType.Comprehension,
                    Page = 8,
                    QuestionText = "What did the boy eat?",
                    Answer = "apples"
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
