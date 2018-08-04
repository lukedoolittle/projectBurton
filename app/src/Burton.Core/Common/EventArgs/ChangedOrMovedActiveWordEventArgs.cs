using System;
using System.Collections.Generic;
using System.Text;
using Burton.Core.Domain;

namespace Burton.Core.Common
{
    public class ChangedOrMovedActiveWordEventArgs
    {
        public WordOnPage NewActiveWord { get; set; }
    }
}
