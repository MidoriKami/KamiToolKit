using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract class LayoutListNode : SimpleComponentNode {

    protected readonly List<NodeBase> NodeList = [];
    private bool suppressRecalculateLayout;

    public IEnumerable<T> GetNodes<T>() where T : NodeBase => NodeList.OfType<T>();

    public IReadOnlyList<NodeBase> Nodes => NodeList;

    public int MaxNodes { get; set; }

    public bool ClipListContents {
        get => NodeFlags.HasFlag(NodeFlags.Clip);
        set {
            if (value) {
                AddFlags(NodeFlags.Clip);
            }
            else {
                RemoveFlags(NodeFlags.Clip);
            }
        }
    }

    public float ItemSpacing { get; set; }

    public float FirstItemSpacing { get; set; }

    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        InternalRecalculateLayout();

        foreach (var node in NodeList) {
            if (node is LayoutListNode subNode) {
                subNode.RecalculateLayout();
            }
        }
    }
    
    protected abstract void InternalRecalculateLayout();

    protected virtual void AdjustNode(NodeBase node) { }

    public void AddNode(IEnumerable<NodeBase> nodes) {
        AddNode(nodes.ToArray());
    }
    
    public void AddNode(params NodeBase?[] items) {
        suppressRecalculateLayout = true;
        
        foreach (var node in items) {
            AddNode(node);
        }
        
        suppressRecalculateLayout = false;
        
        RecalculateLayout();
    }

    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;

        NodeList.Add(node);

        node.AttachNode(this);

        if (MaxNodes >= 1 && NodeList.Count >= MaxNodes) {
            var firstNode = NodeList.First();
            node.NodeId = firstNode.NodeId;
            RemoveNode(firstNode);
        }

        RecalculateLayout();
    }

    public void RemoveNode(params NodeBase[] items) {
        suppressRecalculateLayout = true;
        
        foreach (var node in items) {
            RemoveNode(node);
        }
        
        suppressRecalculateLayout = false;
        
        RecalculateLayout();
    }

    public virtual void RemoveNode(NodeBase node) {
        if (!NodeList.Contains(node)) return;

        node.DetachNode();
        NodeList.Remove(node);
        node.Dispose();

        RecalculateLayout();
    }

    public void AddDummy(float size = 0.0f) {
        var dummyNode = new ResNode {
            Size = new Vector2(size, size),
        };

        AddNode(dummyNode);
    }

    public virtual void Clear() {
        suppressRecalculateLayout = true;
        
        foreach (var node in NodeList.ToList()) {
            RemoveNode(node);
        }

        suppressRecalculateLayout = false;
        
        NodeList.Clear();
        RecalculateLayout();
    }

    public delegate TU CreateNewNode<in T, out TU>(T data) where TU : NodeBase;

    public delegate T GetDataFromNode<out T, in TU>(TU node) where TU : NodeBase;
    
    public bool SyncWithListData<T, TU>(IEnumerable<T> dataList, GetDataFromNode<T?, TU> getDataFromNode, CreateNewNode<T, TU> createNodeMethod)where TU : NodeBase
    {
        suppressRecalculateLayout = true;
    
        bool anythingChanged = false;
    
        var nodesOfType = GetNodes<TU>().ToList();
    
        var dataSet = new HashSet<T>(EqualityComparer<T>.Default);
        foreach (var data in dataList)
            dataSet.Add(data);
    
        var represented = new HashSet<T>(EqualityComparer<T>.Default);
    
        for (int i = 0; i < nodesOfType.Count; i++) {
            TU node = nodesOfType[i];
            T? nodeData = getDataFromNode(node);
    
            if (nodeData is null || !dataSet.Contains(nodeData)) {
                RemoveNode(node);
                anythingChanged = true;
                continue;
            }
    
            represented.Add(nodeData);
        }
    
        foreach (var data in dataSet) {
            if (represented.Contains(data))
                continue;
    
            TU newNode = createNodeMethod(data);
            AddNode(newNode);
            anythingChanged = true;
        }
    
        suppressRecalculateLayout = false;
        RecalculateLayout();
    
        return anythingChanged;
    }

    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        RecalculateLayout();
    }
}
