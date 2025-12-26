using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

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
    
    public ICollection<NodeBase> InitialNodes {
        init => AddNode(value);
    }

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
    
    public bool SyncWithListData<T, TU>(IEnumerable<T> dataList, GetDataFromNode<T?, TU> getDataFromNode, CreateNewNode<T, TU> createNodeMethod) where TU : NodeBase {
        suppressRecalculateLayout = true;
    
        var anythingChanged = false;
        var nodesOfType = GetNodes<TU>().ToList();
        var dataSet = dataList.ToHashSet(EqualityComparer<T>.Default);
        var represented = new HashSet<T>(EqualityComparer<T>.Default);
    
        foreach (var node in nodesOfType) {
            var nodeData = getDataFromNode(node);
    
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
    
            var newNode = createNodeMethod(data);
            AddNode(newNode);
            anythingChanged = true;
        }
    
        suppressRecalculateLayout = false;
        RecalculateLayout();
    
        return anythingChanged;
    }

    public bool SyncWithListDataByKey<T, TU, TKey>(
        IReadOnlyList<T> dataList, 
        Func<T, TKey> getKeyFromData, 
        Func<TU, TKey> getKeyFromNode, 
        Action<TU, T> updateNode, 
        CreateNewNode<T, TU> createNodeMethod, 
        IEqualityComparer<TKey>? keyComparer = null) where TU : NodeBase where TKey : notnull {

        suppressRecalculateLayout = true;

        var anythingChanged = false;
        keyComparer ??= EqualityComparer<TKey>.Default;

        var existing = new List<TU>(capacity: NodeList.Count);
        foreach (var t in NodeList) {
            if (t is TU tu)
                existing.Add(tu);
        }

        var byKey = new Dictionary<TKey, TU>(existing.Count, keyComparer);
        List<TU>? duplicates = null;

        foreach (var node in existing) {
            var key = getKeyFromNode(node);

            if (!byKey.TryAdd(key, node))
                (duplicates ??= new List<TU>(4)).Add(node);
        }

        var desired = new List<TU>(dataList.Count);

        foreach (var data in dataList) {
            var key = getKeyFromData(data);

            if (byKey.TryGetValue(key, out var existingNode)) {
                updateNode(existingNode, data);
                desired.Add(existingNode);
                byKey.Remove(key);
            }
            else {
                var newNode = createNodeMethod(data);
                AddNode(newNode);
                updateNode(newNode, data);

                desired.Add(newNode);
                anythingChanged = true;
            }
        }

        if (byKey.Count != 0) {
            foreach (var kv in byKey) {
                RemoveNode(kv.Value);
                anythingChanged = true;
            }
        }

        if (duplicates is not null) {
            for (var i = 0; i < duplicates.Count; i++) {
                RemoveNode(duplicates[i]);
                anythingChanged = true;
            }
        }

        var desiredCount = desired.Count;
        var j = 0;
        var mismatch = false;

        for (var i = 0; i < NodeList.Count; i++) {
            if (NodeList[i] is TU) {
                if (j >= desiredCount) {
                    mismatch = true;
                    break;
                }

                NodeBase desiredNode = desired[j++];
                if (!ReferenceEquals(NodeList[i], desiredNode)) {
                    NodeList[i] = desiredNode;
                    anythingChanged = true;
                }
            }
        }

        if (!mismatch && j != desiredCount)
            mismatch = true;

        if (mismatch) {
            var firstTuIndex = -1;

            for (var i = 0; i < NodeList.Count; i++) {
                if (NodeList[i] is TU) {
                    firstTuIndex = i;
                    break;
                }
            }

            if (firstTuIndex < 0)
                firstTuIndex = NodeList.Count;

            for (var i = NodeList.Count - 1; i >= 0; i--) {
                if (NodeList[i] is TU)
                    NodeList.RemoveAt(i);
            }

            NodeList.InsertRange(firstTuIndex, desired);
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
