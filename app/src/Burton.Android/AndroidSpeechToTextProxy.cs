using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Media;
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
        private readonly Java.Util.Locale _language;
        private TaskCompletionSource<bool> _permissionSource;

        private string _partialResults = string.Empty;
        private static readonly object PARTIAL_RESULTS_LOCK = new object();

        public event EventHandler<CapturedWordEventArgs> WordCaptured;
        public event EventHandler<WordTimeoutEventArgs> WordTimeout;

        public bool IsListening = false;

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

        public AndroidSpeechToTextProxy(
            ReadingActivity context,
            Java.Util.Locale language)
        {
            _readingActivity = context;
            _language = language;
            CreateIntent();
        }

        private void CreateIntent()
        {
            _speech = SpeechRecognizer.CreateSpeechRecognizer(_readingActivity);
            _speech.SetRecognitionListener(this);
            _speechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            //_speechIntent.PutExtra(RecognizerIntent.ActionRecognizeSpeech, RecognizerIntent.ExtraPreferOffline);
            //_speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 10000);
            //_speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 10000);
            //_speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
            _speechIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            _speechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
        }

        public void StartListening()
        {
            IsListening = true;
            _speech.StartListening(_speechIntent);
        }

        public void StopListening()
        {
            _partialResults = string.Empty;
            IsListening = false;
            _speech.StopListening();
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            _partialResults = string.Empty;

            if (error == SpeechRecognizerError.SpeechTimeout)
            {
                IsListening = false;
                WordTimeout?.Invoke(
                    this,
                    new WordTimeoutEventArgs());
            }
            else
            {
                _speech.Destroy();
                CreateIntent();
                StartListening();
            }
        }

        public void OnPartialResults(Bundle partialResults)
        {
            var matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            if (matches.Count != 0)
            {
                var currentWords = matches[0];
                var newWords = new List<string>();

                lock (PARTIAL_RESULTS_LOCK)
                {
                    newWords = currentWords
                        .GetStringAfterStartingString(_partialResults)
                        .GetNonEmptyTokens();
                    _partialResults = currentWords;
                }

                foreach (var newWord in newWords)
                {
                    WordCaptured?.Invoke(
                        this,
                        new CapturedWordEventArgs { Word = newWord });
                }
            }
        }



        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
            _partialResults = string.Empty;
            IsListening = false;
            WordTimeout?.Invoke(
                this,
                new WordTimeoutEventArgs());
        }

        public void OnReadyForSpeech(Bundle @params)
        {
        }

        public void OnResults(Bundle results)
        {
        }

        public void OnRmsChanged(float rmsdB)
        {
        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
        }
    }
}