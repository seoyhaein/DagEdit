using System;
using System.Data.Common;
using Avalonia;
using Avalonia.Collections;
using DynamicData;

namespace DagEdit
{
    public enum DagItemsType
    {
        // Connection 도 일단 1개 이상일지 생각해야함.
        // Node 역시 3개 이상일지 생각해야함.
        StartNode,
        EndNode,
        RunnerNode,
        Connection
    }

    public class DagItems
    {
        #region Fields

        private DagNode? _nodeItem;

        public DagNode? NodeItem
        {
            get => _nodeItem;
            set => _nodeItem = value;
        }

        private DagConnection? _connectionItem;

        public DagConnection? ConnectionItem
        {
            get => _connectionItem;
            set => _connectionItem = value;
        }

        #endregion

        #region Constructor

        public DagItems()
        {
        }

        #endregion

        #region Methods

        public void CreateDagConnection(Point? sourceAnchor, Guid? sourceNodeId, Point? targetAnchor,
            Guid? targetNodeId)
        {
            _connectionItem = new DagConnection
            {
                ConnectionId = Guid.NewGuid(),
                SourceAnchor = sourceAnchor,
                TargetAnchor = targetAnchor,
                SourceNodeId = sourceNodeId,
                TargetNodeId = targetNodeId,
                DAGItemType = DagItemsType.Connection
            };
        }

        public void CreateDagNode(Point? location)
        {
            _nodeItem = new DagNode
            {
                NodeId = Guid.NewGuid(),
                Location = location,
                DAGItemType = DagItemsType.RunnerNode
            };
        }

        #endregion
    }

    public class DagNode
    {
        public Guid? NodeId { get; set; }
        public Node? NodeInstance { get; set; }

        public Point? Location { get; set; }

        // node 의 anchor 를 나타냄.
        public Point? SourceAnchor { get; set; }
        public Point? TargetAnchor { get; set; }

        // TODO 이름은 추후 생각하자. Source, Target 으로 고치다. 현재는 start, end 로 되어 있음.
        // 이 녀석을 통해서 connection 을 검색할 수 있어야 한다.
        public AvaloniaList<DagConnection> SourceConnections { get; set; } = new AvaloniaList<DagConnection>();
        public AvaloniaList<DagConnection> TargetConnections { get; set; } = new AvaloniaList<DagConnection>();
        public DagItemsType DAGItemType { get; set; }
    }

    // TODO 추후 불필요한 것들 삭제
    public class DagConnection
    {
        public Guid? ConnectionId { get; set; }
        public Connection? ConnectionInstance { get; set; }
        public Guid? SourceNodeId { get; set; }
        public Node? SourceNodeInstance { get; set; }
        public Guid? TargetNodeId { get; set; }
        public Node? TargetNodeInstance { get; set; }

        public DagItemsType DAGItemType { get; set; }

        // connection 의 source, target 을 나타냄.
        public Point? SourceAnchor { get; set; }
        public Point? TargetAnchor { get; set; }
    }
}