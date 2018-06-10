using System.Threading;

namespace Burton.Core.Common
{
    public class ThreadSafeCounter
    {
        private int _i = 0;

        public int Next()
        {
            return Interlocked.Increment(ref _i);
        }

        public void Reset()
        {
            _i = 0;
        }
    }
}
