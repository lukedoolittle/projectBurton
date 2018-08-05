using System;

namespace Burton.Core.Common
{
    public class WordTimeoutEventArgs : EventArgs
    {
        public ReadingActivityMode Purpose { get; set; }
    }
}
