using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Burton.Core.Common
{
    public interface ITextToSpeechProxy
    {
        bool IsSpeaking { get; }
        Task Speak(string message);

        Task InitializeLanguage();
        void OnInit();
    }
}
