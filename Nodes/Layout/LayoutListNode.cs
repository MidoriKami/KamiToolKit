using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
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
            AddNode(node);
        }
    }

    public virtual void AddNode(NodeBase node) {
        NodeList.Add(node);

        node.AttachNode(this);
        node.NodeId = (uint)NodeList.Count + ListBaseId;

        if (MaxNodes >= 1 && NodeList.Count >= MaxNodes) {
            var firstNode = NodeList.First();
            node.NodeId = firstNode.NodeId;
            RemoveNode(firstNode);
        }

        RecalculateLayout();
    }

    public void RemoveNode(params NodeBase[] items) {
        foreach (var node in items) {
            RemoveNode(node);
        }
    }

    public virtual void RemoveNode(NodeBase node) {
        node.DetachNode();
        NodeList.Remove(node);
        node.Dispose();
        RecalculateLayout();
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
}
