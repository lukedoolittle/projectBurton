using System.Collections.Generic;
using System.Linq;

namespace Burton.Core.Domain
{
    public class Viewport
    {
        public Page CurrentPage { get; set; }

        public void HandleNewView(List<WordOnPage> words)
        {
            if (CurrentPage == null)
            {
                CurrentPage = new Page
                {
                    Words = words,
                    ActiveWord = words.First()
                };
            }
            else
            {
                //if we are looking at a different page AND there
                //is no active word (ie the current page is finished)
                //then turn the page
                if (!CurrentPage.ReconcilePage(words) &&
                    CurrentPage.ActiveWord == null)
                {
                    CurrentPage = new Page
                    {
                        Words = words,
                        ActiveWord = words.First()
                    };
                }
            }
        }
    }
}
