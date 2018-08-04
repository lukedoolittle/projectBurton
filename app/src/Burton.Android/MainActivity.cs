using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Views;
using Burton.Core.Common;
using Burton.Core.Domain;
using Tesseract;

namespace Burton.Android
{
    [Activity(Label="Burton.Android", Theme="@android:style/Theme.NoTitleBar", MainLauncher=true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : ReadingActivity
    {
        private TextureView _textureView;
        private SurfaceView _surfaceView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            PermissionRequested += _camera.OnCameraPermissionFinished;
            PermissionRequested += _speechToText.OnMicrophonePermissionFinished;
            _camera.GeneratedPreviewImage += _ocr.CameraGeneratedPreviewImage;

            _speechToText.WordCaptured += (sender, args) =>
            {
                if (_speechToText.IsListening)
                {
                    _reading.HeardSpokenWord(args.Word);
                }
            };

            _speechToText.WordTimeout += (sender, args) =>
            {
                _reading.HeardSpokenWord(string.Empty);
            };

            _speechToText.FinishedSpeaking += (sender, args) =>
            {
                if (args.Purpose != ReadingActivityMode.QuestionAnswering)
                {
                    _reading.StoppedSpeaking();
                }
            };

            _reading.SteppedInRegression += async (sender, args) =>
            {
                await _textToSpeech.Speak(args.Prompt);
                RunOnUiThread(() => {
                    _speechToText.StartListening(_reading.ActivityMode);
                });
            };


            SetContentView(Resource.Layout.Main);

            _textureView = (TextureView)FindViewById(Resource.Id.textureView);
            _textureView.SurfaceTextureListener = this;

            _surfaceView = (SurfaceView)FindViewById(Resource.Id.surfaceview);
            _surfaceView.SetZOrderOnTop(true);
            _surfaceView.Holder.SetFormat(Format.Transparent);
        }

        public override async void OnSurfaceTextureAvailable(
            SurfaceTexture surface, 
            int width, 
            int height)
        {
            base.OnSurfaceTextureAvailable(
                surface, 
                width, 
                height);

            _textureView.LayoutParameters =
                new RelativeLayout.LayoutParams(width, height);

            await _ocr.Initialize();
            await RequestCameraPreview();
            await RequestMicrophoneAccess();
#pragma warning disable 4014
            RequestVoice(); //todo: figure out why we can't make await this call
#pragma warning restore 4014

            _reading.ChangedOrMovedActiveWord += (sender, args) =>
            {
                if (args.NewActiveWord != null && 
                    !_speechToText.IsListening &&
                    !_textToSpeech.IsSpeaking)
                {
                    RunOnUiThread(() => {
                        _speechToText.StartListening(ReadingActivityMode.Reading);
                    });
                }

                if (args.NewActiveWord == null)
                {
                    ClearCanvas();
                    RunOnUiThread(() => {
                        _speechToText.StopListening();
                    });
                }
                else
                {
                    DrawWordUnderline(args.NewActiveWord.Location);
                }
            };

            var rules = new PageRules()
                .AddRule(new BadCharactersRule())
                .AddRule(new LabelDictionaryWordsRule(
                    DictionaryFactory.GetAllWordsForLanguage(
                        AndroidConstants.Language.ToLanguageTag(),
                        this)))
                .AddRule(new ConfidenceRule())
                .AddRule(new SingleCharactersRule())
                .AddRule(new ExtremeSizeBoundingBoxRule())
                .AddRule(new RectangularBoundingBoxRule())
                .AddFinalRule(new FinalRule());

            _ocr.CapturedText += (sender, args) =>
            {
                var validWords = rules.ApplyRules(args.Words);
                if (validWords.Count > 0)
                {
                    _reading.SawNewWords(validWords);
                }
            };
        }

        #region Word Drawing

        private void ClearCanvas()
        {
            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        private void DrawWordUnderline(Rectangle resultLocation)
        {
            var paint = new Paint { Color = Color.Red };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 8f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            canvas.DrawLine(
                resultLocation.X + PerformanceConstants.BoundingBoxXOffset - PerformanceConstants.BoundingBoxWidthInflation / 2, 
                resultLocation.Bottom + PerformanceConstants.BoundingBoxYOffset, 
                resultLocation.X + resultLocation.Width + PerformanceConstants.BoundingBoxXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2,
                resultLocation.Bottom + PerformanceConstants.BoundingBoxYOffset,
                paint);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        private void DrawWordBoundingBoxes(IEnumerable<Rectangle> resultLocations)
        {
            var paint = new Paint { Color = Color.Red };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 2f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            var rectangles = resultLocations.Select(r =>
                new Rect(
                    (int) (r.Left + PerformanceConstants.BoundingBoxXOffset - PerformanceConstants.BoundingBoxWidthInflation/2),
                    (int) (r.Top + PerformanceConstants.BoundingBoxYOffset - PerformanceConstants.BoundingBoxHeightInflation / 2),
                    (int) (r.Right + PerformanceConstants.BoundingBoxXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2),
                    (int) (r.Bottom + PerformanceConstants.BoundingBoxYOffset + PerformanceConstants.BoundingBoxHeightInflation / 2)));
            foreach (var rectangle in rectangles)
            {
                canvas.DrawRect(rectangle, paint);
            }
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        private void DrawWordBoundingBox(Rectangle resultLocation)
        {
            var paint = new Paint {Color = Color.BlueViolet};
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 2f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            var rectangle = new Rect(
                    (int)(resultLocation.Left + PerformanceConstants.BoundingBoxXOffset - PerformanceConstants.BoundingBoxWidthInflation / 2),
                    (int)(resultLocation.Top + PerformanceConstants.BoundingBoxYOffset - PerformanceConstants.BoundingBoxHeightInflation / 2),
                    (int)(resultLocation.Right + PerformanceConstants.BoundingBoxXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2),
                    (int)(resultLocation.Bottom + PerformanceConstants.BoundingBoxYOffset + PerformanceConstants.BoundingBoxHeightInflation / 2));
            canvas.DrawRect(rectangle, paint);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        #endregion Word Drawing

    }
}

