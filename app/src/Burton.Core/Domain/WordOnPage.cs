using Tesseract;

namespace Burton.Core.Domain
{
    public class WordOnPage
    {
        public string Word { get; set; }
        public Rectangle Location { get; set; }

        public float Confidence { get; set; }
    }
}
