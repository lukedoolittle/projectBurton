using System;

namespace Burton.Core.Common
{
    public class PreviewImageEventArgs : EventArgs
    {
        public byte[] Image { get; set; }
    }
}