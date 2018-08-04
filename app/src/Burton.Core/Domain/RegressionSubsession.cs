using System;
using Burton.Core.Common;

namespace Burton.Core.Domain
{
    public class RegressionSubsession
    {
        private readonly LanguageDictionary _dictionary;
        private readonly IPrompts _prompts;
        public string CurrentWord { get; }
        private string _testWord;
        public RegressionState State { get; private set; } = RegressionState.None;
        private int _attempt = 1;

        public bool IsPhonicallyCorrect { get; private set; } = false;
        public bool IsPhonemicallyCorrect { get; private set; } = false;
        public string OriginalAttempt { get; }

        //todo: check parameters for null or empty
        public RegressionSubsession(
            string currentWord,
            string originalAttempt,
            LanguageDictionary dictionary,
            IPrompts prompts)
        {
            CurrentWord = currentWord;
            OriginalAttempt = originalAttempt;
            _dictionary = dictionary;
            _prompts = prompts;
        }

        public string AddressNextStep(string spokenWord)
        {
            if (State == RegressionState.None)
            {
                State = RegressionState.AskingForWord;
                return _prompts.AskForWord;
            }

            if (State == RegressionState.AskingForWord)
            {
                if (spokenWord == CurrentWord)
                {
                    State = RegressionState.None;
                    IsPhonemicallyCorrect = true;
                    IsPhonicallyCorrect = true;
                    return $"{_prompts.Correct} {_prompts.Continuation}";
                }
                else
                {
                    State = RegressionState.CheckingPhonemicCorrectness;
                    _testWord = GetTestWord();
                    return $"{string.Format(_prompts.PhonemicCheck, _testWord)}";
                }
            }

            if (State == RegressionState.CheckingPhonemicCorrectness)
            {
                if (_testWord == CurrentWord && spokenWord == "yes")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    _attempt = 1;
                    IsPhonemicallyCorrect = true;
                    return $"{_prompts.Correct}. {string.Format(_prompts.PhonicCheck, CurrentWord)}";
                }
                else if (_testWord == CurrentWord && spokenWord != "yes")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    _attempt = 1;
                    return $"{_prompts.Fail}. {string.Format(_prompts.PhonicCheck, CurrentWord)}";
                }
                else if (_testWord != CurrentWord && spokenWord == "yes")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    _attempt = 1;
                    return $"{string.Format(_prompts.Correction, CurrentWord)}. {string.Format(_prompts.PhonicCheck, CurrentWord)}";
                }
                else
                {
                    _testWord = _attempt++ > 2 ? 
                        CurrentWord : 
                        GetTestWord();
                    return $"{string.Format(_prompts.PhonemicCheck, _testWord)}";
                }
            }

            if (State == RegressionState.CheckingPhonicCorrectness)
            {
                if (spokenWord == CurrentWord)
                {
                    State = RegressionState.None;
                    IsPhonicallyCorrect = true;
                    return $"{_prompts.Correct}. {_prompts.Continuation}";
                }
                else if (_attempt < 2)
                {
                    _attempt++;
                    return $"{_prompts.Try}. {string.Format(_prompts.PhonicCheck, CurrentWord)}";
                }
                else
                {
                    State = RegressionState.None;
                    return $"{_prompts.Fail}. {_prompts.Continuation}";
                }
            }

            throw new Exception();
        }

        private string GetTestWord()
        {
            return RandomNumberGenerator.RandomBoolean() ? 
                _dictionary.GetRandomWord() : 
                CurrentWord;
        }
    }
}
