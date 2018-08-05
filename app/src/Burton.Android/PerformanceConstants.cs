using System.ComponentModel;

namespace Burton.Android
{
    public static class PerformanceConstants
    {
        public static int Framerate { get; } = 15;
        public static float BoundingGeometryXOffset { get; } = 60;
        public static float BoundingGeometryYOffset { get; } = -20;
        public static float BoundingBoxHeightInflation { get; } = 30;
        public static float BoundingBoxWidthInflation { get; } = 25;
        public static float BoundingCircleWidthInflation { get; } = 100;
        public static float BoundingCircleHeightInflation { get; } = 50;
        public static int PageTurnDelayTimeInMs { get; } = 5000;
    }
}
