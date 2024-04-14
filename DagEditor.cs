using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;

namespace DagEdit
{
    public class DagEditor : SelectingItemsControl, IDisposable
    {
        #region Dependency Properties

        public static readonly StyledProperty<Point> ViewportLocationProperty =
            AvaloniaProperty.Register<DagEditor, Point>(
                nameof(ViewportLocation), Constants.ZeroPoint);

        public Point ViewportLocation
        {
            get => GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        public static readonly StyledProperty<bool> DisablePanningProperty =
            AvaloniaProperty.Register<DagEditor, bool>(nameof(DisablePanning));

        public bool DisablePanning
        {
            get => GetValue(DisablePanningProperty);
            set => SetValue(DisablePanningProperty, value);
        }

        public static readonly DirectProperty<DagEditor, bool> IsSelectingProperty =
            AvaloniaProperty.RegisterDirect<DagEditor, bool>(
                nameof(IsSelecting),
                o => o.IsSelecting);

        private bool _isSelecting;

        public bool IsSelecting
        {
            get => _isSelecting;
            internal set => SetAndRaise(IsSelectingProperty, ref _isSelecting, value);
        }

        public static readonly StyledProperty<bool> EnableRealtimeSelectionProperty =
            AvaloniaProperty.Register<DagEditor, bool>(
                nameof(EnableRealtimeSelection));

        public bool EnableRealtimeSelection
        {
            get => GetValue(EnableRealtimeSelectionProperty);
            set => SetValue(EnableRealtimeSelectionProperty, value);
        }

        public static readonly DirectProperty<DagEditor, Rect> SelectedAreaProperty =
            AvaloniaProperty.RegisterDirect<DagEditor, Rect>(
                nameof(SelectedArea),
                o => o.SelectedArea);

        private Rect _selectedArea;

        public Rect SelectedArea
        {
            get => _selectedArea;
            internal set => SetAndRaise(SelectedAreaProperty, ref _selectedArea, value);
        }

        public static readonly DirectProperty<DagEditor, bool?> IsPreviewingSelectionProperty =
            AvaloniaProperty.RegisterDirect<DagEditor, bool?>(
                nameof(IsPreviewingSelection),
                o => o.IsPreviewingSelection);

        private bool? _isPreviewingSelection;

        public bool? IsPreviewingSelection
        {
            get => _isPreviewingSelection;
            internal set => SetAndRaise(IsPreviewingSelectionProperty, ref _isPreviewingSelection, value);
        }

        public static readonly DirectProperty<DagEditor, bool> IsPanningProperty =
            AvaloniaProperty.RegisterDirect<DagEditor, bool>(
                nameof(IsPanning),
                o => o.IsPanning);

        private bool _isPanning;

        public bool IsPanning
        {
            get => _isPanning;
            protected internal set => SetAndRaise(IsPanningProperty, ref _isPanning, value);
        }

        public static readonly StyledProperty<DataTemplate?> PendingConnectionTemplateProperty =
            AvaloniaProperty.Register<DagEditor, DataTemplate?>(
                nameof(PendingConnectionTemplate));

        public DataTemplate? PendingConnectionTemplate
        {
            get => GetValue(PendingConnectionTemplateProperty);
            set => SetValue(PendingConnectionTemplateProperty, value);
        }

        public static readonly StyledProperty<object?> PendingConnectionProperty =
            AvaloniaProperty.Register<DagEditor, object?>(
                nameof(PendingConnection));

        public object? PendingConnection
        {
            get => GetValue(PendingConnectionProperty);
            set => SetValue(PendingConnectionProperty, value);
        }

        public static readonly StyledProperty<Point?> SourceAnchorProperty =
            AvaloniaProperty.Register<DagEditor, Point?>(nameof(SourceAnchor));

        public Point? SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);
        }

        public static readonly StyledProperty<Point?> TargetAnchorProperty =
            AvaloniaProperty.Register<DagEditor, Point?>(nameof(TargetAnchor));

        public Point? TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }

        // PendingConnection visible 설정에 사용
        public static readonly StyledProperty<bool> IsVisiblePendingConnectionProperty =
            AvaloniaProperty.Register<DagEditor, bool>(
                nameof(IsVisiblePendingConnection));

        public bool IsVisiblePendingConnection
        {
            get => GetValue(IsVisiblePendingConnectionProperty);
            set => SetValue(IsVisiblePendingConnectionProperty, value);
        }

        // TODO 필요 없을 듯 향후 코드 정리 시 지운다.
        public static readonly StyledProperty<Point?> ContextMenuPointProperty =
            AvaloniaProperty.Register<DagEditor, Point?>(nameof(ContextMenuPoint));

        public Point? ContextMenuPoint
        {
            get => GetValue(ContextMenuPointProperty);
            set => SetValue(ContextMenuPointProperty, value);
        }

        #endregion

        #region Fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 이건 connector 에서 올라오는 event
        private EventHandler<PendingConnectionEventArgs>? _connectionStartedHandler;
        private EventHandler<PendingConnectionEventArgs>? _connectionDragHandler;
        private EventHandler<PendingConnectionEventArgs>? _connectionCompleteHandler;
        // 이건 node 에서 올라오는 event
        private EventHandler<ConnectionChangedEventArgs>? _connectionChangedHandler;

        private bool _IsRightBtnClicked;
        //TODO 이거 일단 private 으로 고칠지 고민한다.
        public Dag Dag = new Dag();

        // TODO 아래 변수들 코드 정리시 지운다.
        private bool _isLoaded = true;
        private Canvas? topLayer;
        private DagEditorCanvas? editorCanvas;

        // Panning 관련 포인터 위치 값 
        private Point _previousPointerPosition;
        private Point _currentPointerPosition;

        // TODO 일단 이렇게 남겨 두는데, Menu 디자인시 수정 해야 함.
        private EditorContextFlyout _contextMenu;

        #endregion

        #region Constructors

        public DagEditor()
        {
            DataContext = Dag;
            InitializeSubscriptions();
            _contextMenu = new EditorContextFlyout(this);
            this.Unloaded += (_, _) => this.Dispose();
        }

        #endregion

        #region Event Handlers

        private void InitializeSubscriptions()
        {
            _connectionStartedHandler = HandleConnectionStarted;
            _connectionDragHandler = HandleConnectionDrag;
            _connectionCompleteHandler = HandleConnectionComplete;
            _connectionChangedHandler = HandleConnectionChanged;
           
            Observable.FromEventPattern<PointerPressedEventArgs>(
                    h => this.PointerPressed += h,
                    h => this.PointerPressed -= h)
                .Subscribe(args => HandlePointerPressed(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerEventArgs>(
                    h => this.PointerMoved += h,
                    h => this.PointerMoved -= h)
                .Subscribe(args => HandlePointerMoved(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<PointerReleasedEventArgs>(
                    h => this.PointerReleased += h,
                    h => this.PointerReleased -= h)
                .Subscribe(args => HandlePointerReleased(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<RoutedEventArgs>(
                    h => this.Loaded += h,
                    h => this.Loaded -= h)
                .Subscribe(args => HandleLoaded(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            Observable.FromEventPattern<KeyEventArgs>(
                    h => this.KeyDown += h,
                    h => this.KeyDown -= h)
                .Subscribe(args => HandleKeyDown(args.Sender, args.EventArgs))
                .DisposeWith(_disposables);

            // 이벤트 핸들러 등록
            // PendingConnection
            AddHandler(Connector.PendingConnectionStartedEvent, _connectionStartedHandler);
            AddHandler(Connector.PendingConnectionDragEvent, _connectionDragHandler);
            AddHandler(Connector.PendingConnectionCompletedEvent, _connectionCompleteHandler);
            // Connection Changed
            AddHandler(Node.ConnectionChangedEvent, _connectionChangedHandler);
           
            // 이벤트 핸들러 해제
            _disposables.Add(Disposable.Create(() =>
            {
                // PendingConnection
                RemoveHandler(Connector.PendingConnectionStartedEvent, _connectionStartedHandler);
                RemoveHandler(Connector.PendingConnectionDragEvent, _connectionDragHandler);
                RemoveHandler(Connector.PendingConnectionCompletedEvent, _connectionCompleteHandler);
                // Connection Changed
                RemoveHandler(Node.ConnectionChangedEvent, _connectionChangedHandler);
            }));
        }

        private void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
            if (args.GetCurrentPoint(this).Properties.IsRightButtonPressed && !DisablePanning)
            {
                args.Pointer.Capture(this);
                ContextMenuPoint = args.GetPosition(this);
                _previousPointerPosition = args.GetPosition(this);
                _IsRightBtnClicked = true;
                args.Handled = true;
            }
        }

        private void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
            if (_IsRightBtnClicked)
            {
                _currentPointerPosition = args.GetPosition(this);
                ViewportLocation -=
                    (_currentPointerPosition - _previousPointerPosition) / 1; // Adjust division based on actual zoom level
                _previousPointerPosition = _currentPointerPosition;
                IsPanning = true;
                args.Handled = true;
            }
        }

        private void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
            if (_IsRightBtnClicked)
            {
                args.Handled = true;
                if (IsPanning)
                {
                    IsPanning = false;
                    if (this.Equals(args.Pointer.Captured))
                        args.Pointer.Capture(null);
                    _IsRightBtnClicked = false;
                    return;
                }

                _contextMenu.ShowAt(this, true);
                _IsRightBtnClicked = false;
            }
        }

        private void HandleConnectionStarted(object? sender, PendingConnectionEventArgs args)
        {
            if (args.Source is SourceConnector)
            {
                IsVisiblePendingConnection = true;

                if (args.SourceAnchor.HasValue)
                {
                    SourceAnchor = args.SourceAnchor.Value;
                    // TODO 아래 코드 살펴봐야 함.
                    if (args.Offset.HasValue)
                        TargetAnchor = new Point(SourceAnchor.Value.X + args.Offset.Value.X,
                            SourceAnchor.Value.Y + args.Offset.Value.Y);
                    else TargetAnchor = SourceAnchor;
                }
                else
                {
                    SourceAnchor = null;
                    TargetAnchor = null;
                }

                args.Handled = true;
            }

            Debug.WriteLine("Ok!!!");
        }

        // TODO 중요 여기 반드시 살펴보기
        private void HandleConnectionDrag(object? sender, PendingConnectionEventArgs args)
        {
            // TODO 버그 있음. 살펴보기. 
            if (IsVisiblePendingConnection)
            {
                if (args.Offset.HasValue)
                    TargetAnchor = new Point(args.Offset.Value.X, args.Offset.Value.Y);
                args.Handled = true;
            }
        }

        private void HandleConnectionComplete(object? sender, PendingConnectionEventArgs args)
        {
            args.Handled = true;
            if (args.ConnectedConnector == null || args.SourceAnchor == null || args.TargetAnchor == null)
            {
                IsVisiblePendingConnection = false;
                return;
            }
            Debug.WriteLine("Editor connection end");
            Debug.WriteLine(args.SourceAnchor.Value);
            // 선추가하는 구문.
            Dag.AddDagConnectionItem(args.SourceAnchor, args.SourceNodeId, args.TargetAnchor, args.TargetNodeId);
            IsVisiblePendingConnection = false;
        }

        // TODO 이거 이렇게 하는게 맞는지 살펴보자. 좀더 효율적인 반법이 있을 것 같다.
        private void HandleConnectionChanged(object? sender, ConnectionChangedEventArgs args)
        {
            foreach (var item in Dag.DAGItemsSource)
            {
                // node 도 변경되지만 connection 도 변경됨.
                // dag 데이터 변경은 node 나 connection 에서는 하지 않음. 명심.
                if (item.NodeItem != null)
                {
                    if (item.NodeItem.NodeId == args.NodeId)
                    {
                        // 노드 업데이트
                        item.NodeItem.Location = args.Location;
                        item.NodeItem.SourceAnchor = args.SourceAnchor;
                        item.NodeItem.TargetAnchor = args.TargetAnchor;
                    }
                }

                var connectionItem = item.ConnectionItem;
                if (connectionItem?.ConnectionInstance != null)
                {
                    if (args.SourceAnchor.HasValue && connectionItem.SourceAnchor == args.OldSourceAnchor)
                    {
                        connectionItem.ConnectionInstance.UpdateStart(args.SourceAnchor.Value);
                        connectionItem.SourceAnchor = args.SourceAnchor.Value;
                    }

                    if (args.TargetAnchor.HasValue && connectionItem.TargetAnchor == args.OldTargetAnchor)
                    {
                        connectionItem.ConnectionInstance.UpdateEnd(args.TargetAnchor.Value);
                        connectionItem.TargetAnchor = args.TargetAnchor.Value;
                    }
                }
            }

            args.Handled = true;
        }

        // TODO 코드 정리 할때 이거 필요 없어짐. 삭제, 다만 backup 용으로 기록해 둬야 함.
        private void HandleLoaded(object? sender, RoutedEventArgs args)
        {
            if (_isLoaded)
            {
                editorCanvas = this.GetChildControlByName<DagEditorCanvas>("PART_ItemsHost");
                bool isMatched = Extension.IsCanvasMatched(topLayer, editorCanvas);

                if (!isMatched)
                {
                    Extension.WriteErrorsToFile(
                        "The coordinate systems do not match, causing rendering issues in the application.");
                    throw new InvalidOperationException("The coordinate systems do not match, causing rendering issues in the application.");
                }

                // 한번만 실행되게 만드는 flag
                _isLoaded = false;
            }
        }

        // node 에서 bubble 로 올라옴.
        private void HandleKeyDown(object? sender, KeyEventArgs args)
        {
            // TODO 현재 IsFocused 이 조건이 필요한지는 살펴봐야 함.
            if (args.Source is Node node && EditorGestures.Delete.Matches(args))
            {
                var r = Dag.DelDagNodeItem(node.Id);
                if (!r) Debug.WriteLine("Failed");
                args.Handled = true;
            }
        }

        #endregion

        #region Methods

        // TODO Unload 와 관련 및 GC 관련 해서 생각해보자.
        public void Dispose()
        {
            _disposables.Dispose();
        }

        // 외부에 바인딩해야 해야 함. 입력 파라미터는 없어야 함.
        public void AddNode()
        {
            if (ContextMenuPoint is null) return;
            Dag.AddDagNodeItem(ContextMenuPoint);
        }

        // ContextMenu 말고 MenuFlyout 으로 해보자.

        #endregion

        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            topLayer = e.NameScope.Find<Canvas>("PART_TopLayer");
            if (topLayer == null)
                throw new InvalidOperationException("PART_TopLayer cannot be found in the template.");
        }

        /// <inheritdoc />
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            // TODO switch case 문이 좋을지 고민
            if (item is DagItems dagItems)
            {
                if (dagItems.NodeItem != null)
                {
                    if (dagItems.NodeItem.Location.HasValue)
                    {
                        // 여기서 실제로 SourceAnchor, TargetAnchor 가 생성된다.
                        // TODO 향후에 node 의 참조 해제 해야 한다.
                        var node = new Node(dagItems.NodeItem.Location.Value);
                        dagItems.NodeItem.SourceAnchor = node.SourceAnchor;
                        dagItems.NodeItem.TargetAnchor = node.TargetAnchor;
                        dagItems.NodeItem.NodeInstance = node;
                        // TODO node id update, NodeId 는 반드시 있어야 한다. 이거 nullable 하는 거에 대해서 생각해보자.
                        node.Id = dagItems.NodeItem.NodeId!.Value;
                        return node;
                    }
                }

                if (dagItems.ConnectionItem != null)
                {
                    if (dagItems.ConnectionItem.SourceAnchor.HasValue && dagItems.ConnectionItem.TargetAnchor.HasValue)
                    {
                        var connection = new Connection(dagItems.ConnectionItem.SourceAnchor.Value,
                            dagItems.ConnectionItem.TargetAnchor.Value);

                        dagItems.ConnectionItem.ConnectionInstance = connection;

                        return connection;
                    }
                }
            }

            var emptyControl = new ContentControl { IsVisible = false };
            return emptyControl;
        }
    }
}
