using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class TabbedVerticalListNode : SimpleComponentNode {

    private readonly List<TabbedNodeEntry<NodeBase>> nodeList = [];

    public float TabSize { get; set; } = 18.0f;

    public float ItemVerticalSpacing { get; set; }
    
    public bool FitWidth { get; set; }

    public int TabStep { get; set; }

    // Adds tab amount to any following nodes being added
    public void AddTab(int tabAmount) {
        TabStep += tabAmount;
    }

    // Removes tab amount from any following nodes being added
    public void SubtractTab(int tabAmount) {
        TabStep -= tabAmount;
    }

    public void AddNode(params NodeBase[] nodes) {
        AddNode(0, nodes);
    }

    public void AddNode(int tabIndex, params NodeBase[] nodes) {
        foreach (var node in nodes) {
            AddNode(tabIndex, node);
        }
    }

    public void AddNode(int tabIndex, NodeBase node) {
        nodeList.Add(new TabbedNodeEntry<NodeBase>(node, tabIndex + TabStep));

        node.AttachNode(this);
        node.NodeId = (uint)nodeList.Count + 1;

        RecalculateLayout();
    }

    public void RemoveNode(params NodeBase[] nodes) {
        foreach (var node in nodes) {
            RemoveNode(node);
        }
    }

    public void RemoveNode(NodeBase node) {
        var target = nodeList.FirstOrDefault(item => item.Node == node);
        if (target is null) return;

        target.Node.DetachNode();
        nodeList.Remove(target);
        RecalculateLayout();
    }

    public void Clear() {
        foreach (var nodeEntry in nodeList) {
            nodeEntry.Node.DetachNode();
        }

        nodeList.Clear();
        RecalculateLayout();
    }

    public void RecalculateLayout() {
        var startY = 0.0f;

        foreach (var (node, tab) in nodeList) {
            if (!node.IsVisible) continue;

            node.Y = startY;
            node.X = tab * TabSize;

            if (FitWidth) {
                node.Width = Width - node.X - ItemVerticalSpacing;
                
                // Also update layout of any contained nodes
                if (node is LayoutListNode layoutNode) {
                    layoutNode.RecalculateLayout();
                }
            }
            
            startY += node.Height + ItemVerticalSpacing;
        }

        Height = startY + ItemVerticalSpacing;
    }
}
