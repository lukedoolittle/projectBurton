using System;
using System.Collections.Generic;

namespace Burton.Core.Common
{
    public class CapturedTextEventArgs : EventArgs
    {
        public List<WordOnPage> Text { get; set; }
    }
}
