
namespace Burton.Core.Domain
{
    public class SpeechPerformance
    {
        public string ExpectedWord { get; set; }
        public string ActualWord { get; set; }
        public bool IsPhonemicallyCorrect { get; set; }
        public bool IsPhonicallyCorrect { get; set; }
    }
}
