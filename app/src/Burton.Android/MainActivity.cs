using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Views;
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

            await RequestCameraPreview();
            await RequestMicrophoneAccess();
            RequestVoice();


            //_speechToText.StartListening();

            //var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            //voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            //StartActivityForResult(voiceIntent, 10);


            //_ocr.CapturedText += (sender, args) =>
            //{
            //    foreach (var result in args.Words)
            //    {
            //        DrawWordBoundingBox(result.Location);
            //    }
                
            //};
        }

        private void DrawWordBoundingBox(Rectangle resultLocation)
        {
            //define the paintbrush
            Paint mpaint = new Paint();
            mpaint.Color = Color.Red;
            mpaint.SetStyle(Paint.Style.Stroke);
            mpaint.StrokeWidth = 2f;

            Canvas canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            Rect r = new Rect(
                (int)resultLocation.Left, 
                (int)resultLocation.Top, 
                (int)resultLocation.Right, 
                (int)resultLocation.Bottom);
            canvas.DrawRect(r, mpaint);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }

    }
}

