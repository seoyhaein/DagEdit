using System;
using System.Linq;
using Avalonia;
using Avalonia.Collections;

namespace DagEdit
{
    public class Dag
    {
        private AvaloniaList<DagItems> _dagItemsSource = new AvaloniaList<DagItems>();

        public AvaloniaList<DagItems> DAGItemsSource
        {
            get => _dagItemsSource;
            set => _dagItemsSource = value;
        }

        // TODO 이거 내용 추가적으로 없애주어야함. 테스트로 넣어둠.
        public Dag()
        {
            // 테스트 위해 넣어둠.
            AddDagNodeItem(new Point(10, 10));
            AddDagNodeItem(new Point(200, 280));
            AddDagNodeItem(new Point(300, 300));
        }

        public bool AddDagConnectionItem(Point? source, Guid? sourceNodeId, Point? target, Guid? targetNodeId)
        {
            if (source is null || target is null) return false;

            var newItem = new DagItems();
            newItem.CreateDagConnection(source, sourceNodeId, target, targetNodeId);
            _dagItemsSource.Add(newItem);
            return true;
        }

        public bool AddDagNodeItem(Point? location)
        {
            if (!location.HasValue) return false;

            var newItem = new DagItems();
            newItem.CreateDagNode(location);
            _dagItemsSource.Add(newItem);
            return true;
        }

        // TODO BaseNode 와 Node 의 Dispose 살펴봐야 함.
        // TODO 일단 간단히 Node만 삭제했는데 사실 Connection 도 삭제 해야 한다. 
        public bool DelDagNodeItem(Guid? NodeId)
        {
            // 일부러 여기서는 ?. 안씀. 명시적으로 null 체크 함. 
            var itemToDelete = _dagItemsSource.FirstOrDefault(i => i.NodeItem != null && i.NodeItem.NodeId == NodeId);
            if (itemToDelete != null)
            {
                // NodeInstance 가 null 인 상태에서 삭제하는 것은 위험하다.
                if (itemToDelete.NodeItem!.NodeInstance != null)
                {
                    itemToDelete.NodeItem.NodeInstance.Hide(); // 가시성 제거
                    itemToDelete.NodeItem.NodeInstance.Dispose(true); // Dispose
                    itemToDelete.NodeItem.NodeInstance = null; // GC 를 위한 참조 해제
                    _dagItemsSource.Remove(itemToDelete);
                    return true; // 삭제 성공
                }
            }

            return false; // 매칭되는 아이템이 없어서 삭제 실패
        }
    }
}