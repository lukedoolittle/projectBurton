using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.IO;

namespace Burton.Android
{
    [Activity(Label = "Burton.Android", Theme = "@android:style/Theme.NoTitleBar", MainLauncher = true)]
    public class MainActivity : CameraActivity
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

        public override void OnSurfaceTextureAvailable(
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

            RequestCamera();
        }
    }
}

