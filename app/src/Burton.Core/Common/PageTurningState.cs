using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Burton.Core.Common
{
    public class PageTurningState
    {
        private readonly int _pageTurnTimeInMs;

        public bool IsTurningPage { get; private set; } = false;

        public PageTurningState(int pageTurnTimeInMs)
        {
            _pageTurnTimeInMs = pageTurnTimeInMs;
        }

        public async Task StartTurningPage()
        {
            IsTurningPage = true;
            await Task.Delay(_pageTurnTimeInMs);
            IsTurningPage = false;
        }
    }
}
