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
        private ISurfaceHolder _holder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _textureView = (TextureView)FindViewById(Resource.Id.textureView);
            _textureView.SurfaceTextureListener = this;

            _surfaceView = (SurfaceView)FindViewById(Resource.Id.surfaceview);
            //set to top layer
            _surfaceView.SetZOrderOnTop(true);
            //set the background to transparent
            _surfaceView.Holder.SetFormat(Format.Transparent);
            _holder = _surfaceView.Holder;
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

            _camera.GeneratedPreviewImage += _ocr.CameraGeneratedPreviewImage;


            _ocr.CapturedText += (sender, args) =>
            {
                foreach (var result in args.Text)
                {
                    DrawWordBoundingBox(result.Location);
                }
                _textToSpeech.Speak("Great Job");
            };
        }


        private void DrawWordBoundingBox(Rectangle resultLocation)
        {
            //define the paintbrush
            Paint mpaint = new Paint();
            mpaint.Color = Color.Red;
            mpaint.SetStyle(Paint.Style.Stroke);
            mpaint.StrokeWidth = 2f;

            //draw
            Canvas canvas = _holder.LockCanvas();
            //clear the paint of last time
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            //draw a new one, set your ball's position to the rect here
            Rect r = new Rect(
                (int)resultLocation.Left, 
                (int)resultLocation.Top, 
                (int)resultLocation.Right, 
                (int)resultLocation.Bottom);
            canvas.DrawRect(r, mpaint);
            _holder.UnlockCanvasAndPost(canvas);
        }

        private void FillInBox()
        {

        }

    }
}

