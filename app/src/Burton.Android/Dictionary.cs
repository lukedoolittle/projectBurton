using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Burton.Android
{
    public static class Dictionary
    {
        public static List<string> GetAllEnglishWords()
        {
            string content;
            var assets = MainApplication.CurrentActivity.Assets;
            using (var sr = new StreamReader(assets.Open("popularWords.txt")))
            {
                content = sr.ReadToEnd();
            }

            return content.Split('\n').Select(s => s.TrimEnd('\r')).ToList();
        }
    }
}