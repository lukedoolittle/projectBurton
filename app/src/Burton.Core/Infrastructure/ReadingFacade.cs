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
        private readonly LanguageDictionary _dictionary;
        private readonly IPrompts _prompts;
        private readonly PageTurningState _pageTurningState;
        private RegressionSubsession _regressionSubsession;
        public event EventHandler<ChangedOrMovedActiveWordEventArgs> ChangedOrMovedActiveWord;
        public event EventHandler<SteppedInRegressionEventArgs> SteppedInRegression;

        public ReadingActivityMode ActivityMode => _regressionSubsession == null
            ? ReadingActivityMode.Reading
            : ReadingActivityMode.QuestionAnswering;

        public ReadingFacade(
            Viewport view, 
            ReadingSession readingSession,
            LanguageDictionary dictionary,
            IPrompts prompts,
            int pageTurnDelayTimeInMs)
        {
            _view = view;
            _readingSession = readingSession;
            _dictionary = dictionary;
            _prompts = prompts;
            _pageTurningState = new PageTurningState(pageTurnDelayTimeInMs);
        }

        public void StoppedSpeaking()
        {
            lock (WORD_LOCK)
            {
                //if there is no active word then essentially they have
                //finished the page
                if (_view.CurrentPage.ActiveWord == null)
                {
                    return;
                }

                //if they stopped speaking durring a regression subsession
                //we don't do anything
                if (_regressionSubsession != null)
                {
                    return;
                }

                ConductSubsession(string.Empty);
            }
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

                //if the spoken word is the same as the expected word
                //and we aren't in a regression subsession, record results
                //and advance the word
                if (_view.CurrentPage.ActiveWord.Word == spokenWord && 
                    _regressionSubsession == null)
                {
                    _readingSession.SpeechPerformances.Add(
                        new SpeechPerformance
                        {
                            ExpectedWord = _view.CurrentPage.ActiveWord.Word,
                            ActualWord = spokenWord,
                            IsPhonemicallyCorrect = true,
                            IsPhonicallyCorrect = true
                        });

                    AdvanceWord();
                }
                //if either the spoken word is incorrect or we are in a regression
                //subsession
                else
                {
                    ConductSubsession(spokenWord);
                }
            }
        }

        private void ConductSubsession(string spokenWord)
        {
            if (_regressionSubsession == null)
            {
                _regressionSubsession = new RegressionSubsession(
                    _view.CurrentPage.ActiveWord.Word,
                    spokenWord,
                    _dictionary,
                    _prompts);
            }

            SteppedInRegression?.Invoke(
                this,
                new SteppedInRegressionEventArgs
                {
                    Prompt = _regressionSubsession.AddressNextStep(spokenWord)
                });

            //if the regression subsession is done, record the results,
            //delete the subsession and advance the word
            if (_regressionSubsession.State == RegressionState.None)
            {
                _readingSession.SpeechPerformances.Add(
                    new SpeechPerformance
                    {
                        ExpectedWord = _regressionSubsession.CurrentWord,
                        ActualWord = _regressionSubsession.OriginalAttempt,
                        IsPhonemicallyCorrect = _regressionSubsession.IsPhonemicallyCorrect,
                        IsPhonicallyCorrect = _regressionSubsession.IsPhonicallyCorrect
                    });
                _regressionSubsession = null;
                AdvanceWord();
            }
        }

        private void AdvanceWord()
        {
            _view.AdvanceCurrentWord();

            ChangedOrMovedActiveWord?.Invoke(
                this,
                new ChangedOrMovedActiveWordEventArgs
                {
                    NewActiveWord = _view.CurrentPage.ActiveWord
                });
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
