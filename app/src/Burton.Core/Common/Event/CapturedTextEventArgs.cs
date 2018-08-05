using System;
using System.Collections.Generic;
using Burton.Core.Domain;

namespace Burton.Core.Common
{
    public class CapturedTextEventArgs : EventArgs
    {
        public List<WordOnPage> Words { get; set; }
    }
}
