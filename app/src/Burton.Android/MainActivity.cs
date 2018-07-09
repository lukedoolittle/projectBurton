using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Speech;
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

            string rec = global::Android.Content.PM.PackageManager.FeatureMicrophone;

            if (rec != "android.hardware.microphone")
            {
                throw new Exception("No microphone to record speech");
            }
            //TODO: need to get permissions to the microphone here

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
            //    _textToSpeech.Speak("Great Job");
            //};
        }


        //protected override void OnActivityResult(int requestCode, global::Android.App.Result resultVal, Intent data)
        //{
        //    if (requestCode == 10)
        //    {
        //        if (resultVal == global::Android.App.Result.Ok)
        //        {
        //            var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
        //            if (matches.Count != 0)
        //            {
        //                object a = null;
        //            }
        //        }

        //        //base.OnActivityResult(requestCode, resultVal, data);
        //    }
        //}



        private void DrawWordBoundingBox(Rectangle resultLocation)
        {
            //define the paintbrush
            Paint mpaint = new Paint();
            mpaint.Color = Color.Red;
            mpaint.SetStyle(Paint.Style.Stroke);
            mpaint.StrokeWidth = 2f;

            //draw
            Canvas canvas = _surfaceView.Holder.LockCanvas();
            //clear the paint of last time
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            //draw a new one, set your ball's position to the rect here
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

