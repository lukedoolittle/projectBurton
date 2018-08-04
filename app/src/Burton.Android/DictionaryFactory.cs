using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Burton.Core.Domain;

namespace Burton.Android
{
    public static class DictionaryFactory
    {
        public static LanguageDictionary GetAllWordsForLanguage(
            string language, 
            Activity activity)
        {
            string content;
            var assets = activity.Assets;
            using (var sr = new StreamReader(assets.Open($"popular{language}Words.txt")))
            {
                content = sr.ReadToEnd();
            }

            var words = content.Split('\n').Select(s => s.TrimEnd('\r')).ToList();
            return new LanguageDictionary(words, language);
        }
    }
}