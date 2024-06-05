using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace DagEdit
{
    public class Node : BaseNode
    {
        #region Dependency Properties

        public static readonly StyledProperty<Control?> ParentControlProperty =
            AvaloniaProperty.Register<Node, Control?>(nameof(ParentControl));

        public Control? ParentControl
        {
            get => GetValue(ParentControlProperty);
            set => SetValue(ParentControlProperty, value);
        }
        
        public static readonly DirectProperty<Node, Guid> IdProperty =
            AvaloniaProperty.RegisterDirect<Node, Guid>(
                nameof(Id),
                o => o.Id,
                (o, v) => o.Id = v);

        // TODO 중요. 아래 내용 잊지말자. 기존 Node(GUID) 와 StartNode(int type), EndNode(int type) 는 다른 ID 쳬계를 가져갈려고 한다. 
        // Id 추가 BaseNode 에 않넣는 이유는 StartNode, EndNode 는 다른 ID 체계로 사용할려고 한다.
        private Guid _id;

        public Guid Id
        {
            get => _id;
            set => SetAndRaise(IdProperty, ref _id, value);
        }

        #endregion

        #region fields

        // Node 의 움직임을 위해
        private readonly IDisposable _disposable;
        private readonly TranslateTransform _translateTransform = new TranslateTransform();
        private Point _initialPointerPosition; // 드래그 시작 시 마우스 포인터의 위치
        private Point _initialNodePosition; // 드래그 시작 시 노드의 위치
        private Point _temporaryNewPosition; // 노드의 임시 위치
        private Vector _dragAccumulator; // 드래그 동안의 누적 이동 거리
        // TODO 이름 조정
        private const int GridCellSize = 15; // 그리드 셀 크기, 필요에 따라 조정

        #endregion

        //TODO Node 삭제되는 것도 신경써야 한다.
        #region Constructor

        public Node()
        {
            Focusable = true;
            RenderTransform = _translateTransform;
            _disposable = ParentControlProperty.Changed.Subscribe(HandleParentControlChanged);
        }

        public Node(Point location) : this()
        {
            // 생성자에서만 id 설정하도록 하였음.
            //_id = Guid.NewGuid();
            Location = location;
            (SourceAnchor, TargetAnchor) = FindAnchors(location);
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent<ConnectionChangedEventArgs> ConnectionChangedEvent =
            RoutedEvent.Register<Node, ConnectionChangedEventArgs>(
                nameof(ConnectionChanged),
                RoutingStrategies.Bubble);

        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged
        {
            add => AddHandler(ConnectionChangedEvent, value);
            remove => RemoveHandler(ConnectionChangedEvent, value);
        }

        private void RaiseConnectionChangedEvent(Guid? nodeId, Point? location, Point? sourceAnchor,
            Point? oldSourceAnchor,
            Point? targetAnchor, Point? oldTargetAnchor,
            DagItemsType dagItemsType)
        {
            var args = new ConnectionChangedEventArgs(ConnectionChangedEvent, nodeId, location, sourceAnchor,
                oldSourceAnchor,
                targetAnchor, oldTargetAnchor,
                dagItemsType);
            RaiseEvent(args);
        }

        #endregion

        #region Evnet Handlers

        protected override void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (ParentControl == null)
                throw new InvalidOperationException(
                    "Node cannot move because a DAGlynEditorCanvas parent is not found.");

            if (args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                //Focus();
                args.Pointer.Capture(this);
                Debug.Print("Dragging Start");
                // 드래그 시작 시의 마우스 포인터 위치와 노드의 현재 위치를 저장.
                _initialPointerPosition = args.GetPosition(ParentControl);
                _initialNodePosition = this.Location; // 현재 노드의 위치를 초기 위치로 설정
                // 여기서 초기화 시켜주는 것이 바람직할 것같다.
                _dragAccumulator = new Vector(); // 드래그 누적 거리 초기화
                IsDragging = true;
                args.Handled = true;
            }
        }

        protected override void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (ParentControl == null)
                throw new InvalidOperationException(
                    "Node cannot move because a DAGlynEditorCanvas parent is not found.");

            if (!IsDragging || !this.Equals(args.Pointer.Captured)) return;

            Debug.Print("Dragging...");
            var currentPointerPosition = args.GetPosition(ParentControl);
            // 드래그 시작 위치와 현재 포인터 위치의 차이(delta)를 계산
            var delta = currentPointerPosition - _initialPointerPosition;
            // 노드의 새 위치를 드래그 시작 시 노드 위치 + delta로 계산
            _dragAccumulator += delta;
            // 그리드 크기에 맞추어 효과적인 델타 계산
            var effectiveDelta = new Vector(
                Math.Floor(_dragAccumulator.X / GridCellSize) * GridCellSize,
                Math.Floor(_dragAccumulator.Y / GridCellSize) * GridCellSize);

            if (effectiveDelta != Vector.Zero)
            {
                _translateTransform.X += effectiveDelta.X;
                _translateTransform.Y += effectiveDelta.Y;
                _dragAccumulator -= effectiveDelta; // 적용된 델타만큼 누적 이동 거리 조정
                // 임시 새 위치 계산
                _temporaryNewPosition = new Point(
                    _initialNodePosition.X + _translateTransform.X,
                    _initialNodePosition.Y + _translateTransform.Y);
            }

            _initialPointerPosition = currentPointerPosition; // 포인터 위치 업데이트
            // TODO oldData 기록할 필요 있을듯
            // 아래와 같이 null check는 해야 하지 않을까??
            Point? oldSourceAnchor = SourceAnchor;
            Point? oldTargetAnchor = TargetAnchor;

            (SourceAnchor, TargetAnchor) = FindAnchors(_temporaryNewPosition);
            // TODO 이렇게 event 에 또 event 를 계속 보내는 것 생각해보자.
            RaiseConnectionChangedEvent(_id, this.Location, SourceAnchor, oldSourceAnchor, TargetAnchor,
                oldTargetAnchor,
                DagItemsType.RunnerNode);
            
            args.Handled = true;
        }

        protected override void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (this.ParentControl == null)
                throw new InvalidOperationException(
                    "Node cannot move because a DAGlynEditorCanvas parent is not found.");

            if (sender != null && this.Equals(args.Pointer.Captured) && this.IsDragging)
            {
                Debug.Print("Finish");
                args.Pointer.Capture(null);
                this.IsDragging = false;
                args.Handled = true;
            }
        }
        
        private void HandleParentControlChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is DagEditorCanvas editorCanvas)
                ParentControl = editorCanvas;
            else
                ParentControl = this.GetParentVisualOfType<DagEditorCanvas>();
        }

        #endregion

        #region Methods

        private void NodeMove(Point point)
        {
            Location = point;
            _translateTransform.X = point.X;
            _translateTransform.Y = point.Y;
        }

        #endregion

        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            this.ParentControl = this.GetParentVisualOfType<DagEditorCanvas>();
        }

        public bool CanNodeMove()
        {
            var parentControl = this.GetParentVisualOfType<DagEditorCanvas>();
            if (parentControl != null)
            {
                this.ParentControl = parentControl;
                return true;
            }
            else
            {
                this.ParentControl = null;
                return false;
            }
        }

        public void SetLocation(Point location)
        {
            this.Location = location;
        }

        // TODO 살펴보자.
        public override void Dispose(bool disposing)
        {
            _disposable.Dispose();
            base.Dispose(disposing);
        }

        // TODO (Thinking!!) Node 의 width 와 height 는 고정된걸로 처리한다.
        // Node 의 width, height 는 일단 값을 axaml 에 넣어둔다. 향후 이게 정해지면
        // cs 코드에 넣을 예정임. 현재는 Constants 에 넣어 놓기만 해놓았다.
        private (Point sourceAnchor, Point targetAnchor) FindAnchors(Point location)
        {
            var offset = location;

            var sourceAnchorX = offset.X + Constants.NodeWidth;
            var sourceAnchorY = offset.Y + Constants.NodeHeight / 2;

            var targetAnchorX = offset.X;
            var targetAnchorY = offset.Y + Constants.NodeHeight / 2;

            Point sourceAnchor = new Point(sourceAnchorX, sourceAnchorY);
            Point targetAnchor = new Point(targetAnchorX, targetAnchorY);

            return (sourceAnchor, targetAnchor);
        }
        
    }
}