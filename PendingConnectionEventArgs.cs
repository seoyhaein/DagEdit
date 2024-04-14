using System;
using Avalonia;
using Avalonia.Interactivity;

namespace DagEdit
{
    public class PendingConnectionEventArgs : RoutedEventArgs
    {
        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? connectedConnector, Point? sourceAnchor)
            : base(routedEvent)
        {
            ConnectedConnector = connectedConnector;
            // Anchor 는 InAnchor 와 같아야 한다.
            SourceAnchor = sourceAnchor;
        }

        // TODO 일단 이렇게 추가함.
        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? connectedConnector, Point? sourceAnchor,
            Guid? sourceNodeId,
            Point? targetAnchor, Guid? targetNodeId)
            : base(routedEvent)
        {
            ConnectedConnector = connectedConnector;
            SourceAnchor = sourceAnchor;
            SourceNodeId = sourceNodeId;
            TargetAnchor = targetAnchor;
            TargetNodeId = targetNodeId;
        }

        public PendingConnectionEventArgs(RoutedEvent routedEvent, Connector? connectedConnector, Point? sourceAnchor,
            Vector? offset)
            : base(routedEvent)
        {
            ConnectedConnector = connectedConnector;
            SourceAnchor = sourceAnchor;
            Offset = offset;
        }

        public Connector? ConnectedConnector { get; set; }

        // 이동 거리
        // TODO 이름 다시 생각하자.
        public Vector? Offset { get; set; }

        public Point? SourceAnchor { get; set; }
        public Point? TargetAnchor { get; set; }

        // 일단 이렇게 추가함.
        public Guid? NodeId { get; set; }
        public Guid? SourceNodeId { get; set; }
        public Guid? TargetNodeId { get; set; }
    }
}