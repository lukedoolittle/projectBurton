using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!_tesseract.Initialized)
            {
                await _tesseract.Init(
                    "eng",
                    OcrEngineMode.TesseractCubeCombined);
                _tesseract.SetPageSegmentationMode(PageSegmentationMode.AutoOnly);
                _tesseract.SetBlacklist("/");
            }

            if (await _tesseract.SetImage(e.Image))
            {
                if (!string.IsNullOrEmpty(_tesseract.Text))
                {
                    CapturedText?.Invoke(
                        this,
                        new CapturedTextEventArgs
                        {
                            Words = ParseWordResults(_tesseract.Results(PageIteratorLevel.Word))
                        });
                }
            }
        }

        private static List<WordOnPage> ParseWordResults(IEnumerable<Result> results)
        {
            return results
                .Where(r => r.Confidence >= 0.85f && r.Text != ".")
                .Select(r => new WordOnPage { Word = r.Text.ToLower(), Location = r.Box })
                .ToList();
        }
    }
}
