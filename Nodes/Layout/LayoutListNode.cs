using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public abstract class LayoutListNode : SimpleComponentNode {

    protected readonly List<NodeBase> NodeList = [];

    public IEnumerable<T> GetNodes<T>() where T : NodeBase => NodeList.OfType<T>();

    public IReadOnlyList<NodeBase> Nodes => NodeList;

    protected virtual uint ListBaseId => 1;

    public int MaxNodes { get; set; }

    [JsonProperty] public bool ClipListContents {
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

    [JsonProperty] public float ItemSpacing { get; set; }

    [JsonProperty] public float FirstItemSpacing { get; set; }

    public abstract void RecalculateLayout();

    protected virtual void AdjustNode(NodeBase node) { }

    public void AddNode(params NodeBase[] items) {
        foreach (var node in items) {
            AddNode(node, true);
        }
        
        RecalculateLayout();
    }

    public virtual void AddNode(NodeBase node, bool suppressRecalculateLayout = false) {
        NodeList.Add(node);

        node.AttachNode(this);
        node.NodeId = (uint)NodeList.Count + ListBaseId;

        if (MaxNodes >= 1 && NodeList.Count >= MaxNodes) {
            var firstNode = NodeList.First();
            node.NodeId = firstNode.NodeId;
            RemoveNode(firstNode);
        }

        if (!suppressRecalculateLayout) {
            RecalculateLayout();
        }
    }

    public void RemoveNode(params NodeBase[] items) {
        foreach (var node in items) {
            RemoveNode(node, true);
        }
        
        RecalculateLayout();
    }

    public virtual void RemoveNode(NodeBase node, bool suppressRecalculateLayout = false) {
        node.DetachNode();
        NodeList.Remove(node);
        node.Dispose();

        if (!suppressRecalculateLayout) {
            RecalculateLayout();
        }
    }

    public void AddDummy(float size = 0.0f) {
        var dummyNode = new ResNode {
            Size = new Vector2(size, size), IsVisible = true,
        };

        AddNode(dummyNode);
    }

    public virtual void Clear() {
        foreach (var node in NodeList.ToList()) {
            RemoveNode(node);
        }

        NodeList.Clear();
        RecalculateLayout();
    }

    public delegate TU CreateNewNode<in T, out TU>(T data) where TU : NodeBase;

    public delegate T GetDataFromNode<out T, in TU>(TU node) where TU : NodeBase;
    
    public void SyncWithListData<T, TU>(IList<T> dataList, GetDataFromNode<T?,TU> getDataFromNode, CreateNewNode<T, TU> createNodeMethod) where TU : NodeBase {
        var nodesOfType = GetNodes<TU>().ToList();
        
        var nodesToRemove = nodesOfType.Where(node => !dataList.Any(dataEntry => Equals(dataEntry, getDataFromNode(node)))).ToList();
        
        Log.Verbose($"Removing: {nodesToRemove.Count} Nodes");
        foreach (var node in nodesToRemove) {
            RemoveNode(node, true);
        }
        
        var dataToAdd = dataList.Where(data => !nodesOfType.Any(node => Equals(data, getDataFromNode(node)))).ToList();
        var selectedData = dataToAdd.Select(data => createNodeMethod(data)).ToList();
        
        Log.Verbose($"Adding: {dataToAdd.Count} Nodes");
        foreach (var newNode in selectedData) {
            AddNode(newNode, true);
        }
        
        RecalculateLayout();
    }

    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        RecalculateLayout();
    }
}
