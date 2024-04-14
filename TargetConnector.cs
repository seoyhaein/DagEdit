using Avalonia;
using Avalonia.Input;
using System;

namespace DagEdit
{
    public sealed class TargetConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        #region Dependency Properties

        // connector 에 넣을까 하다가 그냥 여기다 넣음.
        // TODO 여기서 DirectProperty 안 쓰고, AvaloniaProperty 를 쓴 이유는 외부에서 데이터를 설정해야 하기때문이다.
        // 한번 테스트 해보자. (시간날때.)
        public static readonly StyledProperty<Guid> NodeIdProperty =
            AvaloniaProperty.Register<TargetConnector, Guid>(nameof(NodeId));

        public Guid NodeId
        {
            get => GetValue(NodeIdProperty);
            set => SetValue(NodeIdProperty, value);
        }

        #endregion

        static TargetConnector()
        {
            // TODO 향후 이거 주석처리한다.
            // UI 바꿀때, Background 속성 변경.
            //BackgroundProperty.OverrideDefaultValue<InConnector>(new SolidColorBrush(Color.Parse("#4d4d4d")));
            //FocusableProperty.OverrideDefaultValue<InConnector>(true);
            FillProperty.OverrideDefaultValue<TargetConnector>(BrushResources.EndConnectorDefaultFill);
        }

        protected override void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            args.Handled = true;
        }

        protected override void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            args.Handled = true;
        }

        protected override void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            args.Handled = true;
        }
    }
}