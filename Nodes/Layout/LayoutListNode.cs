using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public abstract class LayoutListNode : ResNode {

    protected readonly List<NodeBase> NodeList = [];
    private bool suppressRecalculateLayout;

    public int NavIndex { get; set; }

    public IEnumerable<T> GetNodes<T>() where T : NodeBase => NodeList.OfType<T>();

    public IReadOnlyList<NodeBase> Nodes => NodeList;

    public bool ClipListContents {
        get => NodeFlags.HasFlag(NodeFlags.Clip);
        set {
            if (value) {
                AddNodeFlags(NodeFlags.Clip);
            }
            else {
                RemoveNodeFlags(NodeFlags.Clip);
            }
        }
    }

    public float ItemSpacing { get; set; }

    public float FirstItemSpacing { get; set; }

    public void RecalculateLayout() {
        if (suppressRecalculateLayout) return;

        OnRecalculateLayout();
        OnRecalculateNavigation();

        foreach (var node in NodeList) {
            if (node is LayoutListNode subNode) {
                subNode.RecalculateLayout();
            }
        }
    }

    protected abstract void OnRecalculateLayout();
    protected abstract void OnRecalculateNavigation();

    protected virtual void AdjustNode(NodeBase node) { }

    public ICollection<NodeBase> InitialNodes {
        init => AddNode(value);
    }

    public void AddNode(IEnumerable<NodeBase> nodes) {
        suppressRecalculateLayout = true;
        try {
            foreach (var node in nodes) {
                AddNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
        }
        RecalculateLayout();
    }

    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;

        NodeList.Add(node);

        node.AttachNode(this);

        RecalculateLayout();
    }

    public void RemoveNode(params NodeBase[] items) {
        suppressRecalculateLayout = true;
        try {
            foreach (var node in items) {
                RemoveNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
        }
        RecalculateLayout();
    }

    public virtual void RemoveNode(NodeBase node) {
        if (!NodeList.Contains(node)) return;

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
        try {
            foreach (var node in NodeList.ToList()) {
                RemoveNode(node);
            }
        } finally {
            suppressRecalculateLayout = false;
        }
        RecalculateLayout();
    }

    public void ReorderNodes(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
        RecalculateLayout();
    }
}
