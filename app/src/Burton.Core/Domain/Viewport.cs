using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class Viewport
    {
        public Page CurrentPage { get; private set; }
        private static readonly object VIEW_LOCK = new object();

        public void AdvanceCurrentWord()
        {
            var currentWordIndex = CurrentPage.Words.IndexOf(CurrentPage.ActiveWord);

            CurrentPage.ActiveWord = 
                currentWordIndex == CurrentPage.Words.Count - 1 ? 
                    null : 
                    CurrentPage.Words[currentWordIndex+1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <returns>True if we have a new page, false otherwise</returns>
        public bool AlterPage(List<WordOnPage> words)
        {
            lock (VIEW_LOCK)
            {
                if (words.Count == 0)
                {
                    return false;
                }

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
                    return true;
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
                        return true;
                    }
                    //otherwise if the words are on the current page then adjust all the locations
                    else if (areWordsOnCurrentPage)
                    {
                        for (var i = 0; i < words.Count; i++)
                        {
                            CurrentPage.Words[i].Location = words[i].Location;
                        }
                    }
                    //if we got a superset of the words on the page then adjust the page to
                    //contain the superset
                    else if (CurrentPage.AreWordsSupersetOfCurrentPage(words))
                    {
                        CurrentPage.Words = words;
                        CurrentPage.ActiveWord = words.First(w => w.Word == CurrentPage.ActiveWord.Word);
                    }
                }

                return false;
            }
        }
    }
}
