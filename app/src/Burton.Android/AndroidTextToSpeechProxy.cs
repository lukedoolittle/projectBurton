using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.Speech.Tts;
using Burton.Core.Common;
using Java.Util;

namespace Burton.Android
{
    public class AndroidTextToSpeechProxy :
        UtteranceProgressListener,
        ITextToSpeechProxy
    {
        private readonly ReadingActivity _speechActivity;
        private readonly float _pitch;
        private readonly float _speechRate;
        private readonly string _speechEngine;
        private readonly string _speakerName;
        private readonly Java.Util.Locale _language;
        private readonly TaskCompletionSource<bool> _languageReadySource = 
            new TaskCompletionSource<bool>();
        private TaskCompletionSource<string> _speakingCompleteSource;

        private static object SPEECH_LOCK = new object();

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

        public bool IsSpeaking => _textToSpeech.IsSpeaking;

        public AndroidTextToSpeechProxy()
            : this(null, Locale.English)
        {
        }

        public AndroidTextToSpeechProxy(
            ReadingActivity speechActivity,
            Java.Util.Locale language) :
            this(speechActivity, 
                language, 
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
            //MuteAudio();
            MaxVolume();

            if (!_languageReadySource.Task.IsCompleted)
            {
                _textToSpeech.SetLanguage(_language);
            }

            return _languageReadySource.Task;
        }

        public Task Speak(string message)
        {
            lock (SPEECH_LOCK)
            {
                if (string.IsNullOrEmpty(message) || IsSpeaking)
                {
                    return Task.CompletedTask;
                }

                _speakingCompleteSource = new TaskCompletionSource<string>();

#pragma warning disable 618
                //UnmuteAudio();
                _textToSpeech.Speak(
                    message,
                    QueueMode.Flush,
                    null,
                    Guid.NewGuid().ToString());
                // MuteAudio();
#pragma warning restore 618

                return _speakingCompleteSource.Task;
            }
        }

        public void OnInit()
        {
            //only the default language is supported at current
            if (_textToSpeech.IsLanguageAvailable(_language) == 
                LanguageAvailableResult.Available)
            {
                _textToSpeech.SetVoice(_textToSpeech.Voices.Single(v => v.Name == _speakerName));
                _languageReadySource.SetResult(true);
                _textToSpeech.SetOnUtteranceProgressListener(this);
            }
        }

        private static void MuteAudio()
        {
            var amanager = (AudioManager)MainApplication.CurrentActivity.GetSystemService(Context.AudioService);
#pragma warning disable CS0618 // Type or member is obsolete
            amanager.SetStreamMute(Stream.Music, true);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private static void UnmuteAudio()
        {
            var amanager = (AudioManager)MainApplication.CurrentActivity.GetSystemService(Context.AudioService);
#pragma warning disable 618
            amanager.SetStreamMute(Stream.Music, false);
            var maxVolume = amanager.GetStreamMaxVolume(Stream.Music);
            amanager.SetStreamVolume(Stream.Music, maxVolume, VolumeNotificationFlags.PlaySound);
#pragma warning restore 618
        }

        private static void MaxVolume()
        {
            var amanager = (AudioManager)MainApplication.CurrentActivity.GetSystemService(Context.AudioService);
#pragma warning disable 618
            var maxVolume = amanager.GetStreamMaxVolume(Stream.Music);
            amanager.SetStreamVolume(Stream.Music, maxVolume, VolumeNotificationFlags.PlaySound);
#pragma warning restore 618
        }

        public override void OnDone(string utteranceId)
        {
            _speakingCompleteSource.SetResult(utteranceId);
        }

        public override void OnError(string utteranceId)
        {
        }

        public override void OnStart(string utteranceId)
        {
        }
    }
}