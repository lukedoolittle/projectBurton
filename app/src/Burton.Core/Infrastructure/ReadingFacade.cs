using System;
using System.Collections.Generic;
using Burton.Core.Common;
using Burton.Core.Domain;

namespace Burton.Core.Infrastructure
{
    //TODO: something has to sync the _view page with the _reading page
    
    public class ReadingFacade
    {
        private static readonly object WORD_LOCK = new object();
        private readonly Viewport _view;
        private readonly ReadingSession _readingSession;

        public event EventHandler<ChangedOrMovedActiveWordEventArgs> ChangedOrMovedActiveWord;

        public ReadingFacade(
            Viewport view, 
            ReadingSession readingSession)
        {
            _view = view;
            _readingSession = readingSession;
        }

        public void HeardSpokenWord(string spokenWord)
        {
            lock (WORD_LOCK)
            {
                //if there is no active word we cannot evaluate performance
                //nor advance the current word
                if (_view.CurrentPage.ActiveWord == null)
                {
                    return;
                }

                _readingSession.SpeechPerformances.Add(
                    new SpeechPerformance
                    {
                        ExpectedWord = _view.CurrentPage.ActiveWord.Word,
                        ActualWord = spokenWord,
                        IsPhonemicallyCorrect = _view.CurrentPage.ActiveWord.Word == spokenWord,
                        IsPhonicallyCorrect = _view.CurrentPage.ActiveWord.Word == spokenWord
                    });

                //TODO: in some cases this will not be true (corrective workflow) but for current just advance the word
                _view.AdvanceCurrentWord();

                ChangedOrMovedActiveWord?.Invoke(
                    this,
                    new ChangedOrMovedActiveWordEventArgs
                    {
                        NewActiveWord = _view.CurrentPage.ActiveWord
                    });
            }
        }

        public void SawNewWords(List<WordOnPage> words)
        {
            lock (WORD_LOCK)
            {
                _view.AlterPage(words);

                ChangedOrMovedActiveWord?.Invoke(
                    this,
                    new ChangedOrMovedActiveWordEventArgs
                    {
                        NewActiveWord = _view.CurrentPage.ActiveWord
                    });
            }
        }
    }
}
