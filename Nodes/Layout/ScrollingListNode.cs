using System.Collections.Generic;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// This is a combination of a ScrollingAreaNode and a VerticalListNode for easy layout
/// </summary>
public class ScrollingListNode : SimpleComponentNode {

    public readonly ScrollingAreaNode<VerticalListNode> ListNode;
    
    public ScrollingListNode() {
        ListNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 100.0f,
        };
        ListNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ListNode.Size = Size;
        ListNode.FitToContentHeight();
    }

    public bool FitContents {
        get => ListNode.ContentNode.FitContents;
        set => ListNode.ContentNode.FitContents = value;
    }
    
    public bool FitWidth {
        get => ListNode.ContentNode.FitWidth;
        set => ListNode.ContentNode.FitWidth = value;
    }
    
    public VerticalListAnchor Alignment {
        get => ListNode.ContentNode.Alignment;
        set => ListNode.ContentNode.Alignment = value;
    }

    public bool ClipListContents {
        get => ListNode.ContentNode.ClipListContents;
        set => ListNode.ContentNode.ClipListContents = value;
    }
    
    public float ItemSpacing  {
        get => ListNode.ContentNode.ItemSpacing;
        set => ListNode.ContentNode.ItemSpacing = value;
    }
    
    public float FirstItemSpacing {
        get => ListNode.ContentNode.FirstItemSpacing;
        set => ListNode.ContentNode.FirstItemSpacing = value;
    }

    public ICollection<NodeBase> InitialNodes {
        init => ListNode.ContentNode.AddNode(value);
    }
        
    public IReadOnlyList<NodeBase> Nodes => ListNode.ContentNode.Nodes;
    
    public IEnumerable<T> GetNodes<T>() where T : NodeBase => ListNode.ContentNode.GetNodes<T>();
    
    public void RecalculateLayout() => ListNode.ContentNode.RecalculateLayout();
   
    public void AddNode(IEnumerable<NodeBase> nodes) => ListNode.ContentNode.AddNode(nodes);
    
    public void AddNode(NodeBase? node) => ListNode.ContentNode.AddNode(node);
    
    public void RemoveNode(params NodeBase[] nodes) => ListNode.ContentNode.RemoveNode(nodes);
    
    public void RemoveNode(NodeBase node) => ListNode.ContentNode.RemoveNode(node);
    
    public void AddDummy(float size = 0.0f) => ListNode.ContentNode.AddDummy(size);
    
    public void Clear() => ListNode.ContentNode.Clear();
    
    public delegate TU CreateNewNode<in T, out TU>(T data) where TU : NodeBase;

    public delegate T GetDataFromNode<out T, in TU>(TU node) where TU : NodeBase;
    
    public VerticalListNode VerticalListNode => ListNode.ContentNode;
}
