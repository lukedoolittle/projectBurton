using System;
using System.Collections.Generic;
using System.Linq;
using Burton.Core.Common;
using Burton.Core.Common.Event;
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
        private ComprehensionSubsession _comprehensionSubsession;
        public event EventHandler<ChangedOrMovedActiveWordEventArgs> ChangedOrMovedActiveWord;
        public event EventHandler<SteppedInRegressionEventArgs> SteppedInRegression;
        public event EventHandler<ChangedPageEventArgs> ChangedPage; 

        public ReadingActivityMode ActivityMode => _regressionSubsession != null || _comprehensionSubsession != null
            ? ReadingActivityMode.QuestionAnswering
            : ReadingActivityMode.Reading;

        public bool IsPerformingReadingComprehension => _comprehensionSubsession != null;

        public bool IsTurningPage => _pageTurningState.IsTurningPage;

        public bool IsJustStarting => _view.CurrentPage.IsFirstWordOnPage &&
                                      _view.CurrentPage.PageNumber == 1;

        private bool _canAdjustWordsOnPage => !IsTurningPage && !IsPerformingReadingComprehension;

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
                if (ActivityMode == ReadingActivityMode.QuestionAnswering)
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
                if (_comprehensionSubsession != null)
                {
                    ConductComprehension(spokenWord);
                }

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

        public void SawNewWords(List<WordOnPage> words)
        {
            lock (WORD_LOCK)
            {
                if (!_canAdjustWordsOnPage || words.Count == 0)
                {
                    return;
                }

                if (_view.AlterPage(words))
                {
                    ChangedPage?.Invoke(
                        this,
                        new ChangedPageEventArgs
                        {
                            NewPage = _view.CurrentPage
                        });
                }

                ChangedOrMovedActiveWord?.Invoke(
                    this,
                    new ChangedOrMovedActiveWordEventArgs
                    {
                        NewActiveWord = _view.CurrentPage.ActiveWord
                    });
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

            if (_view.CurrentPage.ActiveWord == null)
            {
                if (!ConductComprehension(string.Empty))
                {
                    _pageTurningState.StartTurningPage();
                }
            }

            ChangedOrMovedActiveWord?.Invoke(
                this,
                new ChangedOrMovedActiveWordEventArgs
                {
                    NewActiveWord = _view.CurrentPage.ActiveWord
                });
        }

        private bool ConductComprehension(string spokenWord)
        {
            if (_comprehensionSubsession != null)
            {
                _readingSession.QuestionPerformances.Add(
                    new QuestionPerformance
                    {
                        Question = _comprehensionSubsession.QuestionText,
                        Answer = spokenWord
                    });

                var prompt = _comprehensionSubsession.IsCorrectAnswer(spokenWord) ? 
                    _prompts.Correct : 
                    $"{_prompts.Fail} {string.Format(_prompts.QuestionCorrection, _comprehensionSubsession.AnswerText)}";

                _comprehensionSubsession = null;
                SteppedInRegression?.Invoke(
                    this,
                    new SteppedInRegressionEventArgs
                    {
                        Prompt = prompt

                    });

                _pageTurningState.StartTurningPage();

                return true;
            }
            else
            {
                var questions = _readingSession
                    .Book
                    .Questions
                    .Where(q => q.Page == _view.CurrentPage.PageNumber);
                if (questions.Any())
                {
                    _comprehensionSubsession = new ComprehensionSubsession(questions.First());
                    SteppedInRegression?.Invoke(
                        this,
                        new SteppedInRegressionEventArgs
                        {
                            Prompt = _comprehensionSubsession.QuestionText
                        });
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}
