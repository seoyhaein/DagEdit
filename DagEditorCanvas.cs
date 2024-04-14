using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DagEdit
{
    public sealed class DagEditorCanvas : Canvas, IDisposable
    {
        #region Dependency Properties

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DagEditorCanvas, Point>(
                nameof(ViewportLocation), Constants.ZeroPoint);

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        #endregion

        #region Fields

        private readonly IDisposable _disposable;

        #endregion

        #region Constructor

        public DagEditorCanvas()
        {
            RenderTransform = new TranslateTransform();
            _disposable = ViewportLocationProperty.Changed.Subscribe(OnViewportLocationChanged);
        }

        #endregion

        //TODO 사이즈에 대한 것은 디버깅해서 살펴보자.
        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
            {
                // ILocatable 인터페이스를 구현하는지 확인
                if (child is ILocatable locatableChild)
                {
                    Point location = locatableChild.Location;
                    child.Arrange(new Rect(location, child.DesiredSize));
                }
                else
                {
                    // ILocatable을 구현하지 않는 경우, 기본 위치나 다른 로직을 사용하여 Arrange를 수행
                    // 기본 위치를 (0, 0)으로 설정
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }

            // TODO finalSize 한번 디버깅해서 봐야 한다.
            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            foreach (var child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return default;
        }

        #region Methods

        private void OnViewportLocationChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Point pointValue)
            {
                if (RenderTransform is TranslateTransform translateTransform)
                {
                    translateTransform.X = -pointValue.X;
                    translateTransform.Y = -pointValue.Y;
                }
            }
        }

        public void Dispose()
        {
            // 관리되는 자원 해제
            _disposable.Dispose();
        }

        #endregion
    }
}