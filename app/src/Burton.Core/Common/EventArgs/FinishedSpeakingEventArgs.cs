using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Common
{
    public class FinishedSpeakingEventArgs : EventArgs
    {
        public ReadingActivityMode Purpose { get; set; }
    }
}
