using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.VisualBasic;

namespace DagEdit
{
    public class BaseNode : ContentControl, IDisposable, ILocatable
    {
        #region Dependency Properties

        public static readonly StyledProperty<Point> LocationProperty =
            AvaloniaProperty.Register<BaseNode, Point>(nameof(Location), Constants.ZeroPoint);

        public Point Location
        {
            get => GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }

        public static readonly StyledProperty<Point?> SourceAnchorProperty =
            AvaloniaProperty.Register<BaseNode, Point?>(nameof(SourceAnchor));

        /// <summary>
        /// StartNode는 OutAnchor 가 있고 EndNode 는 InAnchor 가 있다.
        /// 일반 Node 는 OutAnchor 와 InAnchor 가 있다. 
        /// </summary>
        public Point? SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);
        }

        public static readonly StyledProperty<Point?> TargetAnchorProperty =
            AvaloniaProperty.Register<BaseNode, Point?>(nameof(TargetAnchor));

        public Point? TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }

        #endregion

        #region Fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        protected bool IsDragging = false;

        // Pointer location
        protected Point InitialPointerPosition;
        protected Point PreviousPointerPosition;
        protected Point CurrentPointerPosition;

        #endregion

        #region Constructors

        protected BaseNode()
        {
            InitializeSubscriptions();
        }

        #endregion

        #region event handers

        private void InitializeSubscriptions()
        {
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

            /*Observable.FromEventPattern<KeyEventArgs>(
                    h => this.KeyDown += h, 
                    h => this.KeyDown -= h) 
                .Subscribe(args => HandleKeyDown(args.Sender, args.EventArgs))
                .DisposeWith(_disposables); */
        }

        protected virtual void HandlePointerPressed(object? sender, PointerPressedEventArgs args)
        {
        }

        protected virtual void HandlePointerMoved(object? sender, PointerEventArgs args)
        {
        }

        protected virtual void HandlePointerReleased(object? sender, PointerReleasedEventArgs args)
        {
        }

        /*protected virtual void HandleKeyDown(object? sender, KeyEventArgs args)
        {
        }*/

        protected virtual void HandleLoaded(object? sender, RoutedEventArgs args)
        {
        }

        #endregion

        #region methods

        //TODO Dispose 관련해서 테스트 해봐야 함.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // 종료자 호출 억제
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 관리되는 자원 해제
                _disposables.Dispose();
            }
            // 관리되지 않는 자원 해제 코드가 필요한 경우 여기에 추가
        }

        public void Hide()
        {
            if (IsVisible)
                IsVisible = false;
        }

        // 종료자
        ~BaseNode()
        {
            Dispose(false);
        }

        #endregion
    }
}