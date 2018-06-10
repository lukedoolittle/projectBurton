using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using Android.Util;
using Android.Views;
using Burton.Core.Infrastructure;
using Java.IO;
using Tesseract;
using TinyIoC;

namespace Burton.Android
{
    [Activity(Label="Burton.Android", Theme="@android:style/Theme.NoTitleBar", MainLauncher=true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : ReadingActivity
    {
        private TextureView _textureView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _textureView = new TextureView(this)
            {
                SurfaceTextureListener = this
            };

            SetContentView(_textureView);
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
                new FrameLayout.LayoutParams(width, height);

            await RequestCameraPreview();
            RequestVoice();

            _camera.GeneratedPreviewImage += _ocr.CameraGeneratedPreviewImage;
            _ocr.CapturedText += (sender, args) =>
            {
                _speech.Speak(args.Text);
            };
        }

    }
}

