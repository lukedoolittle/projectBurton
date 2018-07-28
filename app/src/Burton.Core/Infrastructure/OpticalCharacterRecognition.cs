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

        public OpticalCharacterRecognition(ITesseractApi tesseract)
        {
            _tesseract = tesseract ?? throw new ArgumentNullException(nameof(tesseract));
        }

        public async void CameraGeneratedPreviewImage(
            object sender, 
            PreviewImageEventArgs e)
        {
            //todo: probably we should check here for initialization
            if (await _tesseract.SetImage(e.Image))
            {
                if (!string.IsNullOrEmpty(_tesseract.Text))
                {
                    CapturedText?.Invoke(
                        this,
                        new CapturedTextEventArgs
                        {
                            Words = _tesseract
                                .Results(PageIteratorLevel.Word)
                                .Select(r => new WordOnPage
                                {
                                    Word = r.Text.ToLower(),
                                    Location = r.Box,
                                    Confidence = r.Confidence
                                })
                                .ToList()
                        });
                }
            }
        }

        public async Task Initialize()
        {
            if (!_tesseract.Initialized)
            {
                await _tesseract.Init(
                    "eng",
                    OcrEngineMode.TesseractCubeCombined);
                _tesseract.SetPageSegmentationMode(PageSegmentationMode.SingleBlock); //maybe AutoOnly
                _tesseract.SetBlacklist("/");
            }
        }
    }
}
