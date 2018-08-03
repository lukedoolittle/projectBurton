using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.Speech.Tts;

namespace Burton.Android
{
    public class AndroidTextToSpeechProxy
    {
        private readonly ReadingActivity _speechActivity;
        private readonly float _pitch;
        private readonly float _speechRate;
        private readonly string _speechEngine;
        private readonly string _speakerName;
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

        public AndroidTextToSpeechProxy(ReadingActivity speechActivity) :
            this(speechActivity, 
                Java.Util.Locale.English, 
                1.0f, 
                1.0f,
                "com.google.android.tts",
                "en-us-x-sfg#female_2-local")
        {
        }

        public AndroidTextToSpeechProxy(
            ReadingActivity speechActivity,
            Java.Util.Locale language,
            float pitch,
            float speechRate,
            string engine,
            string speakerName)
        {
            _speechActivity = speechActivity ?? throw new ArgumentNullException(nameof(speechActivity));
            _pitch = pitch;
            _speechRate = speechRate;
            _speechEngine = engine;
            _language = language;
            _speakerName = speakerName;
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
            UnmuteAudio();
            _textToSpeech.Speak(
                message,
                QueueMode.Flush,
                null);
            MuteAudio();
#pragma warning restore 618
        }

        public void OnInit(OperationResult status)
        {
            //only the default language is supported at current
            if (_textToSpeech.IsLanguageAvailable(_language) == 
                LanguageAvailableResult.Available)
            {
                _textToSpeech.SetVoice(_textToSpeech.Voices.Single(v => v.Name == _speakerName));
                _languageReadySource.SetResult(true);
            }
        }

        private static void MuteAudio()
        {
            var amanager = (AudioManager)MainApplication.CurrentActivity.GetSystemService(Context.AudioService);
            amanager.SetStreamMute(Stream.Music, true);
        }

        private static void UnmuteAudio()
        {
            var amanager = (AudioManager)MainApplication.CurrentActivity.GetSystemService(Context.AudioService);
            amanager.SetStreamMute(Stream.Music, false);
        }
    }
}