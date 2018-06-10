using System;
using System.Threading.Tasks;
using Android.Speech.Tts;

namespace Burton.Android
{
    public class AndroidSpeechProxy
    {
        private readonly ReadingActivity _speechActivity;
        private readonly float _pitch;
        private readonly float _speechRate;
        private readonly string _speechEngine;
        private readonly Java.Util.Locale _language;
        private readonly TaskCompletionSource<bool> _languageReadySource = 
            new TaskCompletionSource<bool>();

        // ReSharper disable once InconsistentNaming
        private TextToSpeech __textToSpeech;

        // ReSharper disable once InconsistentNaming
        private TextToSpeech _textToSpeech
        {
            get
            {
                if (__textToSpeech == null)
                {
                    __textToSpeech = new TextToSpeech(
                        _speechActivity,
                        _speechActivity,
                        _speechEngine);
                    __textToSpeech.SetPitch(_pitch);
                    __textToSpeech.SetSpeechRate(_speechRate);
                }

                return __textToSpeech;
            }
        }

        public AndroidSpeechProxy(ReadingActivity speechActivity) :
            this(speechActivity, 
                Java.Util.Locale.English, 
                1.0f, 
                1.0f,
                "com.google.android.tts")
        {
        }

        public AndroidSpeechProxy(
            ReadingActivity speechActivity,
            Java.Util.Locale language,
            float pitch,
            float speechRate,
            string engine)
        {
            _speechActivity = speechActivity ?? throw new ArgumentNullException(nameof(speechActivity));
            _pitch = pitch;
            _speechRate = speechRate;
            _speechEngine = engine;
            _language = language;
        }

        public Task InitializeLanguage()
        {
            if (!_languageReadySource.Task.IsCompleted)
            {
                _textToSpeech.SetLanguage(_language);
            }

            return _languageReadySource.Task;
        }

        public void Speak(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

#pragma warning disable 618
            _textToSpeech.Speak(
                message,
                QueueMode.Flush,
                null);
#pragma warning restore 618
        }
        public void OnInit(OperationResult status)
        {
            //only the default language is supported at current
            if (_textToSpeech.IsLanguageAvailable(_language) == 
                LanguageAvailableResult.Available)
            {
                _languageReadySource.SetResult(true);
            }
        }
    }
}