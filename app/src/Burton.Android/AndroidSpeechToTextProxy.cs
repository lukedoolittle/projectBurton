using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Speech.Tts;

namespace Burton.Android
{
    public class AndroidSpeechToTextProxy : 
        Java.Lang.Object, 
        IRecognitionListener, 
        TextToSpeech.IOnInitListener
    {
        private SpeechRecognizer _speech;
        private Intent _speechIntent;
        private readonly ReadingActivity _readingActivity;

        public string Words;


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
            object a = null;
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
            object a = null;
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            Words = error.ToString();
            Restart();
        }

        public void OnEvent(int eventType, Bundle @params)
        {
            object a = null;
        }

        public void OnPartialResults(Bundle partialResults)
        {
            object a = null;
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            object a = null;
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