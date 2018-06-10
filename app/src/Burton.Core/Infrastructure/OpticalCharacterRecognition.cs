using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Burton.Core.Common;
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

            //This should also give confidence and location so use that
            if (await _tesseract.SetImage(e.Image))
            {
                if (_tesseract.Text != null)
                {
                    CapturedText?.Invoke(
                        this,
                        new CapturedTextEventArgs
                        {
                            Text = ParseWordResults(_tesseract.Results(PageIteratorLevel.Word))

                        });
                }
            }
        }

        private static string ParseWordResults(IEnumerable<Result> results)
        {
            var normalizedText = string.Join(
                " ",
                results.Where(r => r.Confidence >= 0.85f).Select(r => r.Text.ToLower()));

            return normalizedText;
        }
    }
}
