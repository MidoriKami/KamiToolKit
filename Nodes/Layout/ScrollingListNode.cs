using System;
using System.Collections.Generic;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// This is a combination of a ScrollingAreaNode and a VerticalListNode for easy layout
/// </summary>
public class ScrollingListNode : SimpleComponentNode {

    private readonly ScrollingAreaNode<VerticalListNode> listNode;
    
    public ScrollingListNode() {
        listNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 100.0f,
        };
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.ContentNode.RecalculateLayout();
        listNode.FitToContentHeight();
    }

    public bool FitContents {
        get => listNode.ContentNode.FitContents;
        set => listNode.ContentNode.FitContents = value;
    }
    
    public bool FitWidth {
        get => listNode.ContentNode.FitWidth;
        set => listNode.ContentNode.FitWidth = value;
    }
    
    public VerticalListAnchor Anchor {
        get => listNode.ContentNode.Anchor;
        set => listNode.ContentNode.Anchor = value;
    }

    public VerticalListAlignment Alignment {
        get => listNode.ContentNode.Alignment;
        set => listNode.ContentNode.Alignment = value;
    }

    public bool ClipListContents {
        get => listNode.ContentNode.ClipListContents;
        set => listNode.ContentNode.ClipListContents = value;
    }
    
    public float ItemSpacing  {
        get => listNode.ContentNode.ItemSpacing;
        set => listNode.ContentNode.ItemSpacing = value;
    }
    
    public float FirstItemSpacing {
        get => listNode.ContentNode.FirstItemSpacing;
        set => listNode.ContentNode.FirstItemSpacing = value;
    }

    public ICollection<NodeBase> InitialNodes {
        init => listNode.ContentNode.AddNode(value);
    }
    
    public bool AutoHideScrollBar {
        get => listNode.AutoHideScrollBar;
        set => listNode.AutoHideScrollBar = value;
    }

    public int ScrollSpeed {
        get => listNode.ScrollSpeed;
        set => listNode.ScrollSpeed = value;
    }
        
    public IReadOnlyList<NodeBase> Nodes => listNode.ContentNode.Nodes;
    
    public IEnumerable<T> GetNodes<T>() where T : NodeBase => listNode.ContentNode.GetNodes<T>();
    
    public void RecalculateLayout() {
        listNode.ContentNode.RecalculateLayout();
        listNode.FitToContentHeight();
    }

    public void AddNode(IEnumerable<NodeBase> nodes) => listNode.ContentNode.AddNode(nodes);
    
    public void AddNode(NodeBase? node) => listNode.ContentNode.AddNode(node);
    
    public void RemoveNode(params NodeBase[] nodes) => listNode.ContentNode.RemoveNode(nodes);
    
    public void RemoveNode(NodeBase node) => listNode.ContentNode.RemoveNode(node);
    
    public void AddDummy(float size = 0.0f) => listNode.ContentNode.AddDummy(size);
    
    public void Clear() => listNode.ContentNode.Clear();
    
    public bool SyncWithListData<T, TU>(IEnumerable<T> dataList, LayoutListNode.GetDataFromNode<T?, TU> getDataFromNode, LayoutListNode.CreateNewNode<T, TU> createNodeMethod) where TU : NodeBase 
        => listNode.ContentNode.SyncWithListData(dataList, getDataFromNode, createNodeMethod);

    public bool SyncWithListDataByKey<T, TU, TKey>(IReadOnlyList<T> dataList, Func<T, TKey> getKeyFromData, Func<TU, TKey> getKeyFromNode, Action<TU, T> updateNode, 
        LayoutListNode.CreateNewNode<T, TU> createNodeMethod, IEqualityComparer<TKey>? keyComparer = null) where TU : NodeBase where TKey : notnull 
        => listNode.ContentNode.SyncWithListDataByKey(dataList, getKeyFromData, getKeyFromNode, updateNode, createNodeMethod, keyComparer);
    
    public void ReorderNodes(Comparison<NodeBase> comparison) => listNode.ContentNode.ReorderNodes(comparison);
    
    public VerticalListNode VerticalListNode => listNode.ContentNode;
}
