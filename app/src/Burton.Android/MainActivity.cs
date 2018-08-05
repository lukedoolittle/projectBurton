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

        private bool _shouldGiveIntroPrompt => _reading.IsJustStarting && !_textToSpeech.IsSpeaking;
        

        protected override async void OnCreate(Bundle savedInstanceState)
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
                RunOnUiThread(() =>
                {
                    string a = args.Prompt;
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

            _textureView.LayoutParameters =
                new RelativeLayout.LayoutParams(width, height);

            await _ocr.Initialize();
            await RequestCameraPreview();
            await RequestMicrophoneAccess();
#pragma warning disable 4014
            RequestVoice(); //todo: figure out why we can't make await this call
#pragma warning restore 4014

            _reading.ChangedPage += async (sender, args) =>
            {
                if (_shouldGiveIntroPrompt)
                {
                    await _textToSpeech.Speak(AndroidConstants.Prompts.Start);
                }

                RunOnUiThread(() =>
                {
                    _speechToText.StartListening(ReadingActivityMode.Reading);
                });
            };

            _reading.ChangedOrMovedActiveWord += (sender, args) =>
            {
                if (args.NewActiveWord == null)
                {
                    ClearCanvas();
                    RunOnUiThread(() =>
                    {
                        _speechToText.StopListening();
                    });
                }
                else
                {
                    if (_reading.ActivityMode == ReadingActivityMode.QuestionAnswering)
                    {
                        DrawWordCircle(args.NewActiveWord.Location);
                    }
                    else
                    {
                        DrawWordUnderline(args.NewActiveWord.Location);
                    }
                }
            };

            _ocr.CapturedText += (sender, args) =>
            {
                var actualWords = rules.ApplyRules(args.Words);
                _reading.SawNewWords(actualWords);
                //DrawWordBoundingBoxes(actualWords.Select(w => w.Location));
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
            paint.StrokeWidth = 12f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            canvas.DrawLine(
                resultLocation.X + PerformanceConstants.BoundingGeometryXOffset - PerformanceConstants.BoundingBoxWidthInflation / 2, 
                resultLocation.Bottom + PerformanceConstants.BoundingGeometryYOffset, 
                resultLocation.X + resultLocation.Width + PerformanceConstants.BoundingGeometryXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2,
                resultLocation.Bottom + PerformanceConstants.BoundingGeometryYOffset,
                paint);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        private void DrawWordCircle(Rectangle resultLocation)
        {
            var paint = new Paint { Color = Color.Red };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 8f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            var rectangle = new RectF(
                (resultLocation.Left + PerformanceConstants.BoundingGeometryXOffset - PerformanceConstants.BoundingCircleWidthInflation / 2),
                (resultLocation.Top + PerformanceConstants.BoundingGeometryYOffset - PerformanceConstants.BoundingCircleHeightInflation / 2),
                (resultLocation.Right + PerformanceConstants.BoundingGeometryXOffset + PerformanceConstants.BoundingCircleWidthInflation / 2),
                (resultLocation.Bottom + PerformanceConstants.BoundingGeometryYOffset + PerformanceConstants.BoundingCircleHeightInflation / 2));

            canvas.DrawOval(rectangle, paint);

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
                new RectF(
                    (r.Left + PerformanceConstants.BoundingGeometryXOffset - PerformanceConstants.BoundingBoxWidthInflation/2),
                    (r.Top + PerformanceConstants.BoundingGeometryYOffset - PerformanceConstants.BoundingBoxHeightInflation / 2),
                    (r.Right + PerformanceConstants.BoundingGeometryXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2),
                    (r.Bottom + PerformanceConstants.BoundingGeometryYOffset + PerformanceConstants.BoundingBoxHeightInflation / 2)));
            foreach (var rectangle in rectangles)
            {
                canvas.DrawRect(rectangle, paint);
            }
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

        //private void DrawWordBoundingBox(Rectangle resultLocation)
        //{
        //    var paint = new Paint {Color = Color.BlueViolet};
        //    paint.SetStyle(Paint.Style.Stroke);
        //    paint.StrokeWidth = 2f;

        //    var canvas = _surfaceView.Holder.LockCanvas();
        //    canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
        //    var rectangle = new Rect(
        //            (int)(resultLocation.Left + PerformanceConstants.BoundingBoxXOffset - PerformanceConstants.BoundingBoxWidthInflation / 2),
        //            (int)(resultLocation.Top + PerformanceConstants.BoundingBoxYOffset - PerformanceConstants.BoundingBoxHeightInflation / 2),
        //            (int)(resultLocation.Right + PerformanceConstants.BoundingBoxXOffset + PerformanceConstants.BoundingBoxWidthInflation / 2),
        //            (int)(resultLocation.Bottom + PerformanceConstants.BoundingBoxYOffset + PerformanceConstants.BoundingBoxHeightInflation / 2));
        //    canvas.DrawRect(rectangle, paint);
        //    _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        //}

        #endregion Word Drawing

    }
}

