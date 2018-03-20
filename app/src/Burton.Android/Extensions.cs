using System.IO;
using Android.Graphics;

namespace Burton.Android
{
    public static class Extensions
    {
        public static void YuvToJpeg(
            this byte[] yuvData, 
            int width, 
            int height, 
            ImageFormatType formatType,
            int quality,
            MemoryStream ms)
        {
            new YuvImage(
                    yuvData, 
                    formatType, 
                    width, 
                    height, 
                    null)
                .CompressToJpeg(
                    new Rect(0, 0, width, height), 
                    quality, 
                    ms);
        }
    }
}