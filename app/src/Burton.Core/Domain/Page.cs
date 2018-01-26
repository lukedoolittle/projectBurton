using System.Collections.Generic;

namespace Burton.Core.Domain
{
    public class Page
    {
        public int PageNumber { get; set; }
        public List<string> Words { get; set; }
    }
}
