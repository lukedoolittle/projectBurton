using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core
{
    public class EnglishPrompts : IPrompts
    {
        public string AskForWord { get; } = "What does this word say?";

        public string Continuation { get; } = "Lets keep going!";

        public string Correct { get; } = "Thats correct!";

        public string Correction { get; } = "This word is {0}";

        public string Fail { get; } = "Good try!";

        public string PhonemicCheck { get; } = "Does this say {0}?";

        public string PhonicCheck { get; } = "Can you say. {0}?";

        public string Start { get; } = "Lets get going!";

        public string Success { get; } = "Great job!";

        public string Try { get; } = "Lets try one more time";

        public string Wait { get; } = "Wait just a minute";
    }
}
