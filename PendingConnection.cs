using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;

namespace DagEdit
{
    // TODO ContentControl 이것으로 할지 TemplateControl 으로 할지 고민.
    public sealed class PendingConnection : ContentControl, IDisposable
    {
        #region Dependency Properties

        // TODO 이름 수정한다.
        // 연결의 시작점
        public static readonly StyledProperty<Point> SourceAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(SourceAnchor));

        // 연결의 끝점
        public static readonly StyledProperty<Point> TargetAnchorProperty =
            AvaloniaProperty.Register<PendingConnection, Point>(nameof(TargetAnchor));

        // 연결 시작의 Connector
        public static readonly StyledProperty<object?> SourceConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(SourceConnector));

        // 연결 끝의 Connector
        public static readonly StyledProperty<object?> TargetConnectorProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(TargetConnector));

        // 미리보기 활성화 여부 정의
        public static readonly StyledProperty<bool> EnablePreviewProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnablePreview));

        // 미리보기 대상 객체 정의
        public static readonly StyledProperty<object?> PreviewTargetProperty =
            AvaloniaProperty.Register<PendingConnection, object?>(nameof(PreviewTarget));

        // 선의 두꼐 정의
        // https://docs.avaloniaui.net/docs/guides/custom-controls/defining-properties
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner<PendingConnection>();

        // 스냅핑 활성화 여부 정의
        public static readonly StyledProperty<bool> EnableSnappingProperty =
            AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnableSnapping));

        // 연결 방향 정의.
        public static readonly StyledProperty<ConnectionDirection> DirectionProperty =
            Connection.DirectionProperty.AddOwner<PendingConnection>();

        // Fill 과 Stroke 를 동시 설정
        public static readonly StyledProperty<IBrush?> SetFillAndStrokeProperty =
            AvaloniaProperty.Register<PendingConnection, IBrush?>(nameof(SetFillAndStroke), defaultValue: null);

        public Point SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);
        }

        public Point TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }

        public object? SourceConnector
        {
            get => GetValue(SourceConnectorProperty);
            set => SetValue(SourceConnectorProperty, value);
        }

        public object? TargetConnector
        {
            get => GetValue(TargetConnectorProperty);
            set => SetValue(TargetConnectorProperty, value);
        }

        public bool EnablePreview
        {
            get => GetValue(EnablePreviewProperty);
            set => SetValue(EnablePreviewProperty, value);
        }

        public object? PreviewTarget
        {
            get => GetValue(PreviewTargetProperty);
            set => SetValue(PreviewTargetProperty, value);
        }

        public bool EnableSnapping
        {
            get => GetValue(EnableSnappingProperty);
            set => SetValue(EnableSnappingProperty, value);
        }

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public ConnectionDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public IBrush? SetFillAndStroke
        {
            get => GetValue(SetFillAndStrokeProperty);
            set => SetValue(SetFillAndStrokeProperty, value);
        }

        // 추가 panning 관련

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

        // TODO 생각하기 readonly 가 필요할까?
        private readonly IDisposable _disposable;

        #endregion

        #region Constructors

        public PendingConnection()
        {
            _disposable = SetFillAndStrokeProperty.Changed.Subscribe(SetFillAndStrokePropertyChanged);

            // panning 관련
            RenderTransform = new TranslateTransform();
            _disposable = ViewportLocationProperty.Changed.Subscribe(OnViewportLocationChanged);

            // TODO axaml 에서 사용시 Dispose 하는 방법에 대해서 생각해보기.
            this.Unloaded += (_, _) => this.Dispose();
        }

        #endregion

        #region Methods

        private void SetFillAndStrokePropertyChanged(AvaloniaPropertyChangedEventArgs value)
        {
            if (value.Sender is Connection connection)
            {
                var brush = value.GetNewValue<IBrush?>(); // value.NewValue 대신 GetNewValue<IBrush?>() 사용
                connection.Fill = brush;
                connection.Stroke = brush;
            }
        }

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