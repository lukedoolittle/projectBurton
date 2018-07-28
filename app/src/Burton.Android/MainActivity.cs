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

            await InitializeOCR();
            await RequestCameraPreview();
            await RequestMicrophoneAccess();
#pragma warning disable 4014
            RequestVoice(); //todo: figure out why we can't make await this call
#pragma warning restore 4014
            
            //_speechToText.StartListening();

            _reading.ChangedActiveWord += (sender, args) =>
            {
                DrawWordBoundingBox(args.NewActiveWord.Location);
            };
        }

        private void DrawWordBoundingBox(Rectangle resultLocation)
        {
            var paint = new Paint {Color = Color.Red};
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 2f;

            var canvas = _surfaceView.Holder.LockCanvas();
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            var rectangle = new Rect(
                (int)resultLocation.Left, 
                (int)resultLocation.Top, 
                (int)resultLocation.Right, 
                (int)resultLocation.Bottom);
            canvas.DrawRect(rectangle, paint);
            _surfaceView.Holder.UnlockCanvasAndPost(canvas);
        }
    }
}

