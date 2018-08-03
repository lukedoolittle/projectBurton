using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class Viewport
    {
        public Page CurrentPage { get; set; }

        public void AdvanceCurrentWord()
        {
            var currentWordIndex = CurrentPage.Words.IndexOf(CurrentPage.ActiveWord);

            CurrentPage.ActiveWord = 
                currentWordIndex == CurrentPage.Words.Count ? 
                    null : 
                    CurrentPage.Words[currentWordIndex+1];
        }

        public void AlterPage(List<WordOnPage> words)
        {
            //if there is no current page then we just started
            //make the words the current page
            if (CurrentPage == null)
            {
                CurrentPage = new Page
                {
                    PageNumber = 1,
                    Words = words,
                    ActiveWord = words.First()
                };
            }
            else
            {
                var areWordsOnCurrentPage = CurrentPage.AreWordsOnCurrentPage(words);

                //if we are looking at a different page AND there
                //is no active word (ie the current page is finished)
                //then turn the page
                if (!areWordsOnCurrentPage &&
                    CurrentPage.ActiveWord == null)
                {
                    CurrentPage = new Page
                    {
                        PageNumber = CurrentPage.PageNumber + 1,
                        Words = words,
                        ActiveWord = words.First()
                    };
                }
                //otherwise if the words are on the current page then adjust all the locations
                else if (areWordsOnCurrentPage)
                {
                    for (var i = 0; i < words.Count; i++)
                    {
                        CurrentPage.Words[i].Location = words[i].Location;
                    }
                }
                else if (CurrentPage.AreWordsSupersetOfCurrentPage(words))
                {
                    CurrentPage.Words = words;
                    CurrentPage.ActiveWord = words.First(w => w.Word == CurrentPage.ActiveWord.Word);
                }
            }
        }
    }
}
