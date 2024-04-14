using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;
using System;

namespace DagEdit
{
    public enum ConnectionOffsetMode
    {
        /// <summary>
        /// 오프셋 없음.
        /// </summary>
        None,

        /// <summary>
        /// 포인트 주변에 원형으로 오프셋 적용.
        /// </summary>
        Circle,

        /// <summary>
        /// 포인트 주변에 사각형 형태로 오프셋 적용.
        /// </summary>
        Rectangle,

        /// <summary>
        /// 포인트 주변에 사각형 형태로 오프셋 적용하되, 모서리에 수직.
        /// </summary>
        Edge,
    }

    //TODO 향후 이름은 수정할 수 있음.
    public enum LineShape
    {
        Line,
        Circuit,
        Quadratic,
    }

    /// <summary>
    /// 연결이 지향하는 방향.
    /// </summary>
    /// 연결의 방향에 대한 정의를 한다.
    public enum ConnectionDirection
    {
        Forward, // 앞쪽 방향
        Backward // 뒤쪽 방향
    }

    /// <summary>
    /// 화살표 머리가 그려지는 위치.
    /// </summary>
    public enum ArrowHeadEnds
    {
        /// <summary>
        /// 시작점에 화살표 머리.
        /// </summary>
        Start,

        /// <summary>
        /// 끝점에 화살표 머리.
        /// </summary>
        End,

        /// <summary>
        /// 양쪽 끝에 화살표 머리.
        /// </summary>
        Both,

        /// <summary>
        /// 화살표 머리 없음.
        /// </summary>
        None
    }

    public class Connection : Shape
    {
        #region Feilds

        private const double BaseOffset = 100d;
        private const double OffsetGrowthRate = 25d;
        private const double Degrees = Math.PI / 180.0d;
        private const double DefaultSpacing = 30d;
        private const double DefaultSAngle = 45d;

        #endregion

        #region Dependency Properties

        public static readonly StyledProperty<Point> SourceProperty =
            AvaloniaProperty.Register<Connection, Point>(nameof(Source));

        public static readonly StyledProperty<Point> TargetProperty =
            AvaloniaProperty.Register<Connection, Point>(nameof(Target));

        public static readonly StyledProperty<Size> SourceOffsetProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(SourceOffset));

        public static readonly StyledProperty<Size> TargetOffsetProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(TargetOffset));

        public static readonly StyledProperty<ConnectionOffsetMode> OffsetModeProperty =
            AvaloniaProperty.Register<Connection, ConnectionOffsetMode>(nameof(OffsetMode), ConnectionOffsetMode.None);

        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            AvaloniaProperty.Register<Connection, ConnectionDirection>(nameof(Direction), ConnectionDirection.Forward);

        public static readonly StyledProperty<ArrowHeadEnds> ArrowHeadEndsProperty =
            AvaloniaProperty.Register<Connection, ArrowHeadEnds>(nameof(ArrowEnds), ArrowHeadEnds.End);

        public static readonly StyledProperty<double> SpacingProperty =
            AvaloniaProperty.Register<Connection, double>(nameof(Spacing), DefaultSpacing);

        public static readonly StyledProperty<Size> ArrowSizeProperty =
            AvaloniaProperty.Register<Connection, Size>(nameof(ArrowSize), defaultValue: Constants.DefaultArrowSize);

        public static readonly StyledProperty<LineShape> LineShapeModeProperty =
            AvaloniaProperty.Register<Connection, LineShape>(nameof(LineShapeMode), LineShape.Line);

        public static readonly StyledProperty<double> AngleProperty =
            AvaloniaProperty.Register<Connection, double>(nameof(Angle), DefaultSAngle);

        public double Angle
        {
            get => GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public LineShape LineShapeMode
        {
            get => GetValue(LineShapeModeProperty);
            set => SetValue(LineShapeModeProperty, value);
        }

        public Point Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Point Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public Size SourceOffset
        {
            get => GetValue(SourceOffsetProperty);
            set => SetValue(SourceOffsetProperty, value);
        }

        public Size TargetOffset
        {
            get => GetValue(TargetOffsetProperty);
            set => SetValue(TargetOffsetProperty, value);
        }

        public ConnectionOffsetMode OffsetMode
        {
            get => GetValue(OffsetModeProperty);
            set => SetValue(OffsetModeProperty, value);
        }

        public ConnectionDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public ArrowHeadEnds ArrowEnds
        {
            get => GetValue(ArrowHeadEndsProperty);
            set => SetValue(ArrowHeadEndsProperty, value);
        }

        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public Size ArrowSize
        {
            get => GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
        }

        #endregion

        #region Constructors

        static Connection()
        {
            // 초기값 설정
            // 이건 추후 생각하자. 이렇게 전역적으로 만들어줘야 할까? 이렇게 하는게 맞을까?
            // 아직까지는 바뀌어야 하는 가능성이 적긴한데 고민이 되긴하다.
            StrokeThicknessProperty.OverrideDefaultValue<Connection>(3);
            StrokeProperty.OverrideDefaultValue<Connection>(Brushes.DodgerBlue);
            FillProperty.OverrideDefaultValue<Connection>(Brushes.DodgerBlue);

            // AffectsGeometry
            AffectsGeometry<Connection>(
                SourceProperty,
                TargetProperty,
                SourceOffsetProperty,
                TargetOffsetProperty,
                OffsetModeProperty,
                DirectionProperty,
                ArrowHeadEndsProperty,
                SpacingProperty,
                ArrowSizeProperty,
                AngleProperty,
                LineShapeModeProperty
            );
        }

        public Connection()
        {
        }

        // 추가
        public Connection(Point start, Point end)
        {
            Source = start;
            Target = end;
        }

        #endregion

        #region Methods

        private void DrawLineGeometry(IGeometryContext context, Point source, Point target)
        {
            var direction = Direction == ConnectionDirection.Forward ? 1d : -1d;
            var spacing = new Vector(Spacing * direction, 0d);
            var arrowOffset = new Vector(ArrowSize.Width * direction, 0d);
            var startPoint = source + spacing;
            var endPoint = Spacing > 0d ? target - arrowOffset : target;

            context.BeginFigure(source, false);

            switch (LineShapeMode)
            {
                case LineShape.Line:
                    context.LineTo(startPoint);
                    context.LineTo(endPoint - spacing);
                    break;
                // TODO 여기 버그 있음.
                case LineShape.Circuit:
                    Point p2 = GetControlPoint(startPoint, endPoint - spacing);
                    context.LineTo(startPoint);
                    context.LineTo(p2);
                    context.LineTo(endPoint - spacing);
                    // 살펴보기
                    //context.LineTo(target);
                    break;

                case LineShape.Quadratic:
                    Vector delta = target - source;
                    double height = Math.Abs(delta.Y);
                    double width = Math.Abs(delta.X);

                    double smooth = Math.Min(BaseOffset, height);
                    double offset = Math.Max(smooth, width / 2d);
                    offset = Math.Min(BaseOffset + Math.Sqrt(width * OffsetGrowthRate), offset);

                    var controlPoint = new Vector(offset * direction, 0d);
                    context.CubicBezierTo(startPoint + controlPoint, endPoint - controlPoint, endPoint);
                    break;
            }

            context.LineTo(endPoint);
            context.EndFigure(false);
        }

        private Point GetControlPoint(Point source, Point target)
        {
            Vector delta = target - source;
            double tangent = Math.Tan(Angle * Degrees); // 각도에 따른 탄젠트 값 계산

            double dx = Math.Abs(delta.X);
            double dy = Math.Abs(delta.Y);

            double slopeWidth = dy / tangent; // 수직 거리와 탄젠트를 이용해 수평 길이 계산
            double slopeHeight = dx * tangent; // 수평 거리와 탄젠트를 이용해 수직 길이 계산

            if (dx > slopeWidth)
            {
                // 수평 거리가 계산된 수평 길이보다 큰 경우
                return delta.X > 0d
                    ? new Point(target.X - slopeWidth, source.Y) // 오른쪽 방향일 경우
                    : new Point(source.X - slopeWidth, target.Y); // 왼쪽 방향일 경우
            }

            if (dy > slopeHeight)
            {
                // 수직 거리가 계산된 수직 길이보다 큰 경우
                if (delta.Y > 0d)
                {
                    // 위쪽 방향일 경우
                    return delta.X < 0d
                        ? new Point(source.X, target.Y - slopeHeight) // 왼쪽 위 방향
                        : new Point(target.X, source.Y + slopeHeight); // 오른쪽 위 방향
                }
                else if (delta.X < 0d)
                {
                    // 아래쪽 방향일 경우 (왼쪽 아래 방향)
                    return new Point(source.X, target.Y + slopeHeight);
                }
            }

            // 아래쪽 방향일 경우 (오른쪽 아래 방향)
            return new Point(target.X, source.Y - slopeHeight);
        }

        private void DrawArrowGeometry(IGeometryContext context, Point source, Point target,
            ConnectionDirection arrowDirection = ConnectionDirection.Forward)
        {
            var (from, to) = GetArrowHeadPoints(source, target, arrowDirection);

            context.BeginFigure(target, true);
            context.LineTo(from);
            context.LineTo(to);
            // 주석 지우지 말것!
            // Stroke 색상 가져오기 Avalonia 에서는 필요 없음.
            // var strokeColor = Stroke is SolidColorBrush strokeBrush ? strokeBrush.Color : Colors.Black;
            // Fill 속성에 Stroke 색상 적용
            //context.Fill(new SolidColorBrush(strokeColor));

            context.EndFigure(true);
        }

        private void DrawRectGeometry(IGeometryContext context, Point source)
        {
            // TODO 사각형의 크기를 정의 (예: 10x10 픽셀)
            double size = 10;
            Rect rect = new Rect(source.X - size / 2, source.Y - size / 2, size, size);

            // 사각형 그리기
            context.BeginFigure(rect.TopLeft, isFilled: true);
            context.LineTo(new Point(rect.TopRight.X, rect.TopRight.Y));
            context.LineTo(new Point(rect.BottomRight.X, rect.BottomRight.Y));
            context.LineTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y));
            context.LineTo(rect.TopLeft);
            context.EndFigure(true);
        }

        private (Point From, Point To) GetArrowHeadPoints(Point source, Point target,
            ConnectionDirection arrowDirection)
        {
            var headWidth = ArrowSize.Width;
            var headHeight = ArrowSize.Height;
            Point from;
            Point to;

            // Spacing이 1보다 작은 경우, 화살표의 머리 부분을 각도를 사용하여 계산.
            if (Spacing < 1d)
            {
                var delta = source - target;
                var angle = Math.Atan2(delta.Y, delta.X);
                var sinT = Math.Sin(angle);
                var cosT = Math.Cos(angle);

                from = new Point(target.X + (headWidth * cosT - headHeight * sinT),
                    target.Y + (headWidth * sinT + headHeight * cosT));
                to = new Point(target.X + (headWidth * cosT + headHeight * sinT),
                    target.Y - (headHeight * cosT - headWidth * sinT));
            }
            // Spacing이 1보다 큰 경우, 화살표의 머리 부분을 방향에 따라 계산.
            else
            {
                var direction = arrowDirection == ConnectionDirection.Forward ? 1d : -1d;
                from = new Point(target.X - headWidth * direction, target.Y + headHeight);
                to = new Point(target.X - headWidth * direction, target.Y - headHeight);
            }

            return (from, to);
        }

        private (Vector SourceOffset, Vector TargetOffset) GetOffset()
        {
            Vector delta = Target - Source;
            Vector delta2 = Source - Target;

            return OffsetMode switch
            {
                ConnectionOffsetMode.Rectangle => (GetRectangleModeOffset(delta, SourceOffset),
                    GetRectangleModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.Circle => (GetCircleModeOffset(delta, SourceOffset),
                    GetCircleModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.Edge => (GetEdgeModeOffset(delta, SourceOffset),
                    GetEdgeModeOffset(delta2, TargetOffset)),
                ConnectionOffsetMode.None => (Constants.ZeroVector, Constants.ZeroVector),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Vector GetEdgeModeOffset(Vector delta, Size offset)
        {
            var xOffset = Math.Min(Math.Abs(delta.X) / 2d, offset.Width) * Math.Sign(delta.X);
            var yOffset = Math.Min(Math.Abs(delta.Y) / 2d, offset.Height) * Math.Sign(delta.Y);

            return new Vector(xOffset, yOffset);
        }

        private static Vector GetCircleModeOffset(Vector delta, Size offset)
        {
            if (delta.SquaredLength > 0d)
                delta.Normalize();

            return new Vector(delta.X * offset.Width, delta.Y * offset.Height);
        }

        private static Vector GetRectangleModeOffset(Vector delta, Size offset)
        {
            if (delta.SquaredLength > 0d)
                delta.Normalize();

            var angle = Math.Atan2(delta.Y, delta.X);

            if (offset.Width * 2d * Math.Abs(delta.Y) < offset.Height * 2d * Math.Abs(delta.X))
            {
                var x = Math.Sign(delta.X) * offset.Width;
                var y = Math.Tan(angle) * x;
                return new Vector(x, y);
            }
            else
            {
                var y = Math.Sign(delta.Y) * offset.Height;
                var x = 1.0d / Math.Tan(angle) * y;
                return new Vector(x, y);
            }
        }

        public void UpdateConnection(Point start, Point end)
        {
            Source = start;
            Target = end;
        }

        public void UpdateStart(Point source)
        {
            Source = source;
        }

        public void UpdateEnd(Point target)
        {
            Target = target;
        }

        #endregion

        /// <inheritdoc />
        protected override Geometry CreateDefiningGeometry()
        {
            var geometry = new StreamGeometry();
            using var context = geometry.Open();
            context.SetFillRule(FillRule.EvenOdd);

            // 오프셋 계산 및 소스와 타겟 점 업데이트
            var (sourceOffset, targetOffset) = GetOffset();
            var source = Source + sourceOffset;
            var target = Target + targetOffset;

            // TODO source와 target이 같은 경우, 작은 사각형을 그림
            // 일단 사각형을 그렸는데, 없어도 상관없다면 DrawRectGeometry 주석처리 하면 됨.
            if (source == target)
            {
                DrawRectGeometry(context, source);
                return geometry;
            }

            // 선 그리기
            DrawLineGeometry(context, source, target);

            // 화살표 그리기 (화살표 크기가 0이 아닌 경우) None, Default 는 생략했음.
            if (ArrowSize.Width != 0d && ArrowSize.Height != 0d)
            {
                switch (ArrowEnds)
                {
                    case ArrowHeadEnds.Start:
                    {
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Backward);
                        // TODO 일단 사각형을 그렸는데, 없어도 상관없다면 DrawRectGeometry 주석처리 하면 됨.
                        DrawRectGeometry(context, source);
                        break;
                    }

                    case ArrowHeadEnds.End:
                    {
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Forward);
                        // TODO 일단 사각형을 그렸는데, 없어도 상관없다면 DrawRectGeometry 주석처리 하면 됨.
                        DrawRectGeometry(context, source);
                        break;
                    }

                    case ArrowHeadEnds.Both:
                        DrawArrowGeometry(context, source, target, ConnectionDirection.Forward);
                        DrawArrowGeometry(context, target, source, ConnectionDirection.Backward);

                        break;
                }
            }

            return geometry;
        }
    }
}