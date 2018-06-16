using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Speech.Tts;
using Android.Views;
using Burton.Core.Infrastructure;
using Tesseract;
using TinyIoC;
using Result = Android.App.Result;

namespace Burton.Android
{
    [Activity(Label = "ReadingActivity")]
    public class ReadingActivity : 
        Activity, 
        TextureView.ISurfaceTextureListener,
        TextToSpeech.IOnInitListener
    {
        private SurfaceTexture _surface;
        protected readonly AndroidCameraProxy _camera;
        protected readonly AndroidTextToSpeechProxy _textToSpeech;
        protected readonly AndroidSpeechToTextProxy _speechToText;
        protected readonly OpticalCharacterRecognition _ocr;

        public ReadingActivity()
        {
            _camera = new AndroidCameraProxy(this, 25);
            _textToSpeech = new AndroidTextToSpeechProxy(this);
            //_speechToText = new AndroidSpeechToTextProxy(this);
            _ocr = new OpticalCharacterRecognition(TinyIoCContainer.Current.Resolve<ITesseractApi>());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public async Task RequestCameraPreview()
        {
            if (await _camera.CanAccessCamera())
            {
                _camera.StartCameraPreview(_surface);
            }
            else
            {
                throw new Exception("Cannot get camera permissions");
            }
        }

        public Task RequestVoice()
        {
            return _textToSpeech.InitializeLanguage();
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            _camera.OnCameraPermissionFinished(
                requestCode, 
                permissions,
                grantResults);
        }

        #region ISurfaceTextureListener

        public virtual void OnSurfaceTextureAvailable(
            SurfaceTexture surface, 
            int width, 
            int height)
        {
            _surface = surface;
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            _camera.ShutdownCameraPreview();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(
            SurfaceTexture surface, 
            int width, 
            int height)
        {
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        #endregion ISurfaceTextureListener

        #region IOnInitListener

        public void OnInit(OperationResult status)
        {
            _textToSpeech.OnInit(status);
        }

        #endregion IOnInitListener
    }
}