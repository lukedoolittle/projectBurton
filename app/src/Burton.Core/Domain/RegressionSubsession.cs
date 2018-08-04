using System;
using Burton.Core.Common;

namespace Burton.Core.Domain
{
    public class RegressionSubsession
    {
        private readonly LanguageDictionary _dictionary;
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
            LanguageDictionary dictionary)
        {
            CurrentWord = currentWord;
            OriginalAttempt = originalAttempt;
            _dictionary = dictionary;
        }

        public string AddressNextStep(string spokenWord)
        {
            if (State == RegressionState.None)
            {
                State = RegressionState.AskingForWord;
                return Prompts.AskForWord;
            }

            if (State == RegressionState.AskingForWord)
            {
                if (spokenWord == CurrentWord)
                {
                    State = RegressionState.None;
                    IsPhonemicallyCorrect = true;
                    IsPhonicallyCorrect = true;
                    return $"{Prompts.Correct} {Prompts.Continuation}";
                }
                else
                {
                    State = RegressionState.CheckingPhonemicCorrectness;
                    SetTestWord();
                    return $"{string.Format(Prompts.PhonemicCheck, _testWord)}";
                }
            }

            if (State == RegressionState.CheckingPhonemicCorrectness)
            {
                if (_testWord == CurrentWord && spokenWord == "yes")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    IsPhonemicallyCorrect = true;
                    return $"{Prompts.Correct} {string.Format(Prompts.PhonicCheck, CurrentWord)}";
                }
                else if (_testWord == CurrentWord && spokenWord == "no")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    return $"{Prompts.Fail} {string.Format(Prompts.PhonicCheck, CurrentWord)}";
                }
                else if (_testWord != CurrentWord && spokenWord == "yes")
                {
                    State = RegressionState.CheckingPhonicCorrectness;
                    return $"{string.Format(Prompts.Correction, CurrentWord)} {string.Format(Prompts.PhonicCheck, CurrentWord)}";
                }
                else
                {
                    SetTestWord();
                    return $"{string.Format(Prompts.PhonemicCheck, _testWord)}";
                }
            }

            if (State == RegressionState.CheckingPhonicCorrectness)
            {
                if (spokenWord == CurrentWord)
                {
                    State = RegressionState.None;
                    IsPhonicallyCorrect = true;
                    return $"{Prompts.Correct} {Prompts.Continuation}";
                }
                else if (_attempt < 2)
                {
                    return $"{Prompts.Try} {string.Format(Prompts.PhonicCheck, CurrentWord)}";
                }
                else
                {
                    State = RegressionState.None;
                    return $"{Prompts.Fail} {Prompts.Continuation}";
                }
            }

            throw new Exception();
        }

        private void SetTestWord()
        {
            _testWord = RandomNumberGenerator.RandomBoolean() ?
                _dictionary.GetRandomWord() :
                CurrentWord;
        }
    }
}
