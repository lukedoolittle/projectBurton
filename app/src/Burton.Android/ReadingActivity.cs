using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Speech.Tts;
using Android.Views;
using Burton.Core.Common;
using Burton.Core.Domain;
using Burton.Core.Infrastructure;
using Tesseract;
using TinyIoC;
using Permission = Android.Content.PM.Permission;

namespace Burton.Android
{
    [Activity(Label = "ReadingActivity")]
    public class ReadingActivity : 
        Activity, 
        TextureView.ISurfaceTextureListener,
        TextToSpeech.IOnInitListener
    {
        private SurfaceTexture _surface;
        protected ReadingFacade _reading;
        private AndroidCameraProxy _camera;
        private AndroidTextToSpeechProxy _textToSpeech;
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
                    StartTime = DateTimeOffset.Now
                },
                DictionaryFactory.GetAllWordsForLanguage(
                    AndroidConstants.Language.ToLanguageTag(),
                    this));

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
                if (_speechToText.IsListening)
                {
                    _reading.HeardSpokenWord(string.Empty);
                }
            };

            _reading.SteppedInRegression += (sender, args) =>
            {
                _textToSpeech.Speak(args.Prompt);
            };
        }

        public Task InitializeOcr()
        {
            return _ocr.Initialize();
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

        public void OnInit(OperationResult status)
        {
            _textToSpeech.OnInit(status);
        }

        #endregion IOnInitListener

    }
}