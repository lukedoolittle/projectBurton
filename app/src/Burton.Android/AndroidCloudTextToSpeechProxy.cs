using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Util;
using CognitiveServicesTTS;
using Java.Lang;
using Encoding = Android.Media.Encoding;
using Exception = System.Exception;
using Stream = System.IO.Stream;

namespace Burton.Android
{
    public class AndroidCloudTextToSpeechProxy
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
        public async Task Speak(string textToSpeak)
        {
            var auth = new Authentication(
                AUTHENTICATION_URI, 
                _apiKey);
            var accessToken = auth.GetAccessToken();

            var cortana = new Synthesize();
            cortana.OnAudioAvailable += PlayAudio;

            await cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
            {
                RequestUri = new Uri(REQUEST_URI),
                Text = textToSpeak,
                VoiceType = Gender.Female,
                Locale = _locale,
                VoiceName = VOICE,
                OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + accessToken,
            });
        }

        private async void PlayAudio(
            object sender, 
            GenericEventArgs<Stream> e)
        {
            var memoryStream = new MemoryStream();
            e.EventData.CopyTo(memoryStream);
            var audioBytes = memoryStream.ToArray();

            try
            {

                var audioTrack = new AudioTrack(
                    global::Android.Media.Stream.Music,
                    16000,
                    ChannelOut.Mono, 
                    Encoding.Pcm16bit,
                    audioBytes.Length,
                    AudioTrackMode.Stream);


                for (int i = 0; i < 2; i++)
                {
                    try
                    {

                        audioTrack.Play();
                        await audioTrack.WriteAsync(audioBytes, 0, audioBytes.Length);

                    }
                    catch (IllegalStateException illEx)
                    {
                        Log.Debug("StaveApp", $"Unable to initialize audio exception {illEx.Message}");
                    }

                    await Task.Delay(2000);
                }

                audioTrack.Release();
                audioTrack.Dispose();

            }
            catch (Exception exception)
            {
                Log.Debug("StaveApp", $"Exception in Android.PlayWordImplementation: {exception.Message}");

            }
        }
    }
}