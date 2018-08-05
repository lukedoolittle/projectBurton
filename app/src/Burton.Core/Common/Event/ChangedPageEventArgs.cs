using System;
using Burton.Core.Domain;

namespace Burton.Core.Common.Event
{
    public class ChangedPageEventArgs : EventArgs
    {
        public Page NewPage { get; set; }
    }
}
