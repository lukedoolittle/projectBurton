using System;
using System.Linq;
using System.Threading.Tasks;
using Burton.Core.Common;
using Burton.Core.Domain;
using Tesseract;

namespace Burton.Core.Infrastructure
{
    public class OpticalCharacterRecognition
    {
        public event EventHandler<CapturedTextEventArgs> CapturedText;
        private readonly ITesseractApi _tesseract;
        private bool _isProcessingImage = false;
        private static object _processingImageLock = new object();

        public OpticalCharacterRecognition(ITesseractApi tesseract)
        {
            _tesseract = tesseract ?? throw new ArgumentNullException(nameof(tesseract));
        }

        public async void CameraGeneratedPreviewImage(
            object sender, 
            PreviewImageEventArgs e)
        {
            lock (_processingImageLock)
            {
                if (_isProcessingImage)
                {
                    return;
                }
                else
                {
                    _isProcessingImage = true;
                }
            }

            await _tesseract.SetImage(e.Image).ConfigureAwait(false);

            _isProcessingImage = false;

            if (!string.IsNullOrEmpty(_tesseract.Text))
            {
                var words = _tesseract
                    .Results(PageIteratorLevel.Word)
                    .Where(r => !string.IsNullOrEmpty(r.Text))
                    .Select(r => new WordOnPage
                    {
                        Word = r.Text.ToLower(),
                        Location = r.Box,
                        Confidence = r.Confidence
                    })
                    .ToList();
                CapturedText?.Invoke(
                    this,
                    new CapturedTextEventArgs
                    {
                        Words = words
                    });
            }
        }

        public async Task Initialize()
        {
            if (!_tesseract.Initialized)
            {
                await _tesseract.Init(
                    "eng",
                    OcrEngineMode.TesseractOnly);
                _tesseract.SetPageSegmentationMode(PageSegmentationMode.SingleBlock); //maybe AutoOnly
                _tesseract.SetWhitelist("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890");
            }
        }
    }
}
