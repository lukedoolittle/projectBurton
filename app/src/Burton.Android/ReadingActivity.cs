using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Speech.Tts;
using Android.Views;
using Burton.Core.Common;
using Burton.Core.Data;
using Burton.Core.Domain;
using Burton.Core.Infrastructure;
using Tesseract;
using TinyIoC;
using Permission = Android.Content.PM.Permission;

namespace Burton.Android
{
    //todo: reduce this to just permissions and move the rest into MainActivity
    [Activity(Label = "ReadingActivity")]
    public class ReadingActivity : 
        Activity, 
        TextureView.ISurfaceTextureListener,
        TextToSpeech.IOnInitListener
    {
        private SurfaceTexture _surface;
        protected ReadingFacade _reading;
        protected AndroidCameraProxy _camera;
        protected AndroidTextToSpeechProxy _textToSpeech;
        protected AndroidSpeechToTextProxy _speechToText;
        protected OpticalCharacterRecognition _ocr;

        public event EventHandler<PermissionResultEventArgs> PermissionRequested;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _camera = new AndroidCameraProxy(
                this,
                PerformanceConstants.Framerate);
            _textToSpeech = new AndroidTextToSpeechProxy(
                this,
                AndroidConstants.Language);
            _speechToText = new AndroidSpeechToTextProxy(
                this,
                AndroidConstants.Language);
            _ocr = new OpticalCharacterRecognition(
                TinyIoCContainer.Current.Resolve<ITesseractApi>());
            _reading = new ReadingFacade(
                new Viewport(),
                new ReadingSession
                {
                    StartTime = DateTimeOffset.Now,
                    Book = Books.TheGivingTree
                },
                DictionaryFactory.GetAllWordsForLanguage(
                    AndroidConstants.Language.ToLanguageTag(),
                    this),
                AndroidConstants.Prompts,
                PerformanceConstants.PageTurnDelayTimeInMs);
        }

        #region Permissions

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

        public async Task RequestMicrophoneAccess()
        {
            if (!await _speechToText.CanAccessMicrophone())
            {
                throw new Exception("Cannot get microphone permissions");
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
            PermissionRequested?.Invoke(
                this,
                new PermissionResultEventArgs
                {
                    RequestCode = requestCode,
                    Permissions = permissions,
                    GrantResults = grantResults.Select(r => r == Permission.Granted ? 
                        Burton.Core.Common.Permission.Granted : 
                        Burton.Core.Common.Permission.Denied).ToArray()
                });
        }

        #endregion Permissions

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

        //todo: figure out how to assign this manually so that AndroidTextToSpeechProxy
        //can recieve this directly
        public void OnInit(OperationResult status)
        {
            _textToSpeech.OnInit(status);
        }

        #endregion IOnInitListener

    }
}