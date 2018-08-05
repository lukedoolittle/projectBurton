using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;
using Burton.Core.Common;
using CognitiveServicesTTS;
using Encoding = Android.Media.Encoding;
using Stream = System.IO.Stream;

namespace Burton.Android
{
    //todo: cache the results because a lot of the speech is repeated.
    public class AndroidCloudTextToSpeechProxy : ITextToSpeechProxy
    {
        private const string REQUEST_URI = "https://westus.tts.speech.microsoft.com/cognitiveservices/v1";
        private const string AUTHENTICATION_URI = "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private const string VOICE = "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)";
        // "Microsoft Server Speech Text to Speech Voice (en-US, Guy24KRUS)",
        // "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",
        private readonly string _apiKey;
        private readonly string _locale;

        public AndroidCloudTextToSpeechProxy(
            string apiKey,
            string locale)
        {
            _apiKey = apiKey;
            _locale = locale;
        }

        public bool IsSpeaking { get; private set; } = false;

        public Task Speak(string textToSpeak)
        {
            var auth = new Authentication(
                AUTHENTICATION_URI, 
                _apiKey);
            var accessToken = auth.GetAccessToken();

            var cortana = new Synthesize();
            var taskCompletion = new TaskCompletionSource<bool>();

            cortana.OnAudioAvailable += async (sender, args) =>
            {
                IsSpeaking = true;
                var memoryStream = new MemoryStream();
                args.EventData.CopyTo(memoryStream);
                var audioBytes = memoryStream.ToArray();

                var audioTrack = new AudioTrack(
                    global::Android.Media.Stream.Music,
                    24000,
                    ChannelOut.Mono,
                    Encoding.Pcm16bit,
                    audioBytes.Length,
                    AudioTrackMode.Stream);

                await audioTrack.WriteAsync(audioBytes, 0, audioBytes.Length);
                audioTrack.Play();

                await Task.Delay(ApproximateSpeakingTime(textToSpeak));

                audioTrack.Release();
                audioTrack.Dispose();
                IsSpeaking = false;
                taskCompletion.SetResult(true);
            };

            cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
            {
                RequestUri = new Uri(REQUEST_URI),
                Text = textToSpeak,
                VoiceType = Gender.Female,
                Locale = _locale,
                VoiceName = VOICE,
                OutputFormat = AudioOutputFormat.Riff24Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + accessToken,
            });

            return taskCompletion.Task;
        }

        //to meet the interface definition
        public Task InitializeLanguage()
        {
            return Task.CompletedTask;
        }

        //to meet the interface definition
        public void OnInit()
        {
        }

        private static int ApproximateSpeakingTime(string textToSpeak)
        {
            return textToSpeak.Replace(" ", String.Empty).Length * 142;
        }
    }
}