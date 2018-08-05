using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Domain
{
    public class ComprehensionSubsession
    {
        private readonly Question _question;

        public string QuestionText => _question.QuestionText;
        public string AnswerText => _question.Answer;

        public ComprehensionSubsession(Question question)
        {
            _question = question;
        }

        public bool IsCorrectAnswer(string givenAnswer)
        {
            return givenAnswer == _question.Answer;
        }
    }
}
