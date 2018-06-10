using System;
using System.IO;
using Android.Graphics;

namespace Burton.Android
{
    public static class Extensions
    {
        public static byte[] ImageToJpeg(
            this byte[] yuvData, 
            int width, 
            int height, 
            ImageFormatType formatType,
            int quality = 80)
        {
            if (formatType == ImageFormatType.Nv21)
            {
                var ms = new MemoryStream();

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
                return ms.ToArray();
            }
            else
            {
                throw new Exception("Unrecognized image type");
            }
        }
    }
}