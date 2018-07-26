using Burton.Core.Domain;

namespace Burton.Core.Infrastructure
{
    //TODO: something has to sync the _view page with the _reading page
    
    public class ReadingFacade
    {
        private readonly Viewport _view;
        private readonly Reading _readingSession;

        public ReadingFacade(
            Viewport view, 
            Reading readingSession)
        {
            _view = view;
            _readingSession = readingSession;
        }

        public void HeardSpokenWord(string spokenWord)
        {
            _readingSession.SpeechPerformances.Add(
                new SpeechPerformance
                {
                    ExpectedWord = _view.CurrentPage.ActiveWord.Word,
                    ActualWord = spokenWord,
                    IsPhonemicallyCorrect = _view.CurrentPage.ActiveWord.Word == spokenWord,
                    IsPhonicallyCorrect = _view.CurrentPage.ActiveWord.Word == spokenWord
                });

            //TODO: in some cases this will not be true but for current just advance the word
            _view.AdvanceCurrentWord();
        }
    }
}
