using Avalonia;

namespace DagEdit
{
    public static class Constants
    {
        public const double AppliedThreshold = 12d * 12d;
        public static readonly Point ZeroPoint = new Point(0, 0);
        public static readonly Vector ZeroVector = new(0d, 0d);
        public static readonly Size DefaultArrowSize = new Size(7, 6);

        // TODO 아래 값을 가지고 Anchor 를 미리 계산한다.
        public const double NodeWidth = 200d;
        public const double NodeHeight = 124d;
    }
}