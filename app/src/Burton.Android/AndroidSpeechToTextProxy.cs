using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Speech.Tts;
using Burton.Core.Common;
using Permission = Android.Content.PM.Permission;

namespace Burton.Android
{
    public class AndroidSpeechToTextProxy : 
        Java.Lang.Object, 
        IRecognitionListener, 
        TextToSpeech.IOnInitListener
    {
        private const string AUDIO_PERMISSION = Manifest.Permission.RecordAudio;
        private const int PERMISSION_REQUEST_CODE = 1;

        private SpeechRecognizer _speech;
        private Intent _speechIntent;
        private readonly ReadingActivity _readingActivity;
        private TaskCompletionSource<bool> _permissionSource;

        public string Words;

        public Task<bool> CanAccessMicrophone()
        {
            if (global::Android.Content.PM.PackageManager.FeatureMicrophone != "android.hardware.microphone")
            {
                throw new Exception("No microphone to record speech");
            }

            if ((int)Build.VERSION.SdkInt >= 23 &&
                _readingActivity.CheckSelfPermission(AUDIO_PERMISSION) != (int)Permission.Granted)
            {
                _permissionSource = new TaskCompletionSource<bool>();

                _readingActivity.RequestPermissions(
                    new[] { AUDIO_PERMISSION },
                    PERMISSION_REQUEST_CODE);

                return _permissionSource.Task;
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        public void OnMicrophonePermissionFinished(
            object sender,
            PermissionResultEventArgs args)
        {
            if (args.RequestCode == PERMISSION_REQUEST_CODE)
            {
                if (args.GrantResults.Length == 1 &&
                    args.GrantResults[0] == Core.Common.Permission.Granted)
                {
                    _permissionSource.SetResult(true);
                }
                else
                {
                    _permissionSource.SetResult(false);
                }
            }
        }

        public AndroidSpeechToTextProxy(ReadingActivity context)
        {
            _readingActivity = context;
            Words = "";
            _speech = SpeechRecognizer.CreateSpeechRecognizer(_readingActivity);
            _speech.SetRecognitionListener(this);
            _speechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            //_speechIntent.PutExtra(RecognizerIntent.ActionRecognizeSpeech, RecognizerIntent.ExtraPreferOffline);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            _speechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
        }

        void Restart()
        {
            _speech.Destroy();
            _speech = SpeechRecognizer.CreateSpeechRecognizer(_readingActivity);
            _speech.SetRecognitionListener(this);
            _speechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            //_speechIntent.PutExtra(RecognizerIntent.ActionRecognizeSpeech, RecognizerIntent.ExtraPreferOffline);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            _speechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            StartListening();
        }

        public void StartListening()
        {
            _speech.StartListening(_speechIntent);
        }

        public void StopListening()
        {
            _speech.StopListening();
        }

        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            Words = error.ToString();
            Restart();
        }

        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnPartialResults(Bundle partialResults)
        {
            object a = null;
        }

        public void OnReadyForSpeech(Bundle @params)
        {
        }

        public void OnResults(Bundle results)
        {

            var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (matches == null)
                Words = "Null";
            else if (matches.Count != 0)
                Words = matches[0];
            else
                Words = "";

            //do anything you want for the result
        }

        public void OnRmsChanged(float rmsdB)
        {

        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            object a = null;
        }
    }
}