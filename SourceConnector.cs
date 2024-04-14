using Avalonia.Input;
using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DagEdit
{
    public sealed class SourceConnector : Connector
    {
        protected override Type StyleKeyOverride => typeof(Connector);

        #region Dependency Properties

        // connector 에 넣을까 하다가 그냥 여기다 넣음.
        // TODO 여기서 DirectProperty 안 쓰고, AvaloniaProperty 를 쓴 이유는 외부에서 데이터를 설정해야 하기때문이다.
        // 한번 테스트 해보자. (시간날때.)
        public static readonly StyledProperty<Guid> NodeIdProperty =
            AvaloniaProperty.Register<SourceConnector, Guid>(nameof(NodeId));

        public Guid NodeId
        {
            get => GetValue(NodeIdProperty);
            set => SetValue(NodeIdProperty, value);
        }

        #endregion

        #region Constructor

        static SourceConnector()
        {
            FillProperty.OverrideDefaultValue<SourceConnector>(BrushResources.StartConnectorDefaultFill);
        }

        #endregion

        #region Fields

        private Connector? elementUnderPointer;

        #endregion

        #region Evnet Handlers

        protected override void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                args.Pointer.Capture(this);
                Debug.Print("Pointer Pressed");
                PreviousConnector = this;
                IsPointerPressed = true;
                args.Handled = true;
                RaiseConnectionStartEvent(this, Anchor);
            }
        }

        // Panning 하면 좌표계가 달라짐.
        protected override void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (sender == null || !IsPointerPressed || PreviousConnector == null) return;

            // PART_TopLayer 는 DAGlynEditor.axaml 에 있다. 이녀석이 없으면 기능을 하지 않는다.
            // 이거 나중에 바인딩으로 연결하자.
            //var parent = this.GetParentVisualByName<Canvas>("PART_TopLayer");
            // TODO 이벤트 올때 마다 계산하게 하면 좀 힘들 듯. 이거 계선 해야 함.
            var parent = this.GetParentVisualByName<Canvas>("PART_ItemsHost");
            if (parent == null) return;
            var currentPosition = args.GetPosition(parent);

            // 마우스 이동중 새로운 Connector 에 들어가면 null 이 아님.
            // 계속 업데이트 됨.
            // TODO 이 메서드 최적화 시킬 필요 있을 듯.
            elementUnderPointer = parent.GetControlUnderPointer<Connector>(currentPosition);
            RaiseConnectionDragEvent(this, Anchor, (Vector)currentPosition);
            args.Handled = true;
        }

        // 현재 좀 단순화 했음.
        protected override void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            IsPointerPressed = false; // 마우스 눌림 상태 해제
            args.Pointer.Capture(null);
            args.Handled = true;
            PreviousConnector = null;

            if (elementUnderPointer is TargetConnector okConnector)
                RaiseConnectionCompletedEvent(okConnector, Anchor, NodeId, okConnector.Anchor, okConnector.NodeId);
            else
                RaiseConnectionCompletedEvent(null, null, null, null, null);
        }

        #endregion

        #region Methods

        // Raise events
        /// <summary>
        /// OutConnector 에서 Connection 시작할때 PendingConnection 에 필요한 데이터 이벤트로 전달.
        /// </summary>
        /// <param name="connector">전달되는 값은 OutConnector 여야 함.</param>
        /// <param name="anchor">이벤트 발생시텀에서의 위치값. (이후 조정되어야 함.)</param>
        protected override void RaiseConnectionStartEvent(Connector? connector, Point? anchor)
        {
            var args = new PendingConnectionEventArgs(PendingConnectionStartedEvent, connector, anchor);
            RaiseEvent(args);
        }

        /// <summary>
        /// OutConnector 에서 Dragging 할때 PendingConnection 에 필요한 데이터 이벤트로 전달.
        /// </summary>
        /// <param name="connector">전달되는 값은 OutConnector 여야 함.</param>
        /// <param name="anchor">connection 이 시작될때의 위치값이다.</param>
        /// <param name="offset">이동 Vector 값 (이후 조정되어야 함.)</param>
        protected override void RaiseConnectionDragEvent(Connector? connector, Point? anchor, Vector? offset)
        {
            var args = new PendingConnectionEventArgs(PendingConnectionDragEvent, connector, anchor, offset);
            RaiseEvent(args);
        }

        protected override void RaiseConnectionCompletedEvent(Connector? connector, Point? startAnchor, Guid? inNodeId,
            Point? endAnchor, Guid? outNodeId)
        {
            var args = new PendingConnectionEventArgs(PendingConnectionCompletedEvent, connector, startAnchor, inNodeId,
                endAnchor, outNodeId);
            RaiseEvent(args);
        }

        #endregion

        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            Debug.WriteLine(Anchor.ToString());
        }
    }
}