using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Burton.Android
{
    public static class Dictionary
    {
        public static List<string> GetAllEnglishWords()
        {
            string content;
            var assets = MainApplication.CurrentActivity.Assets;
            using (var sr = new StreamReader(assets.Open("popularWords")))
            {
                content = sr.ReadToEnd();
            }

            return content.Split('\n').ToList();
        }
    }
}