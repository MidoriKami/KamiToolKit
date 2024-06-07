using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.NodeBase;

namespace KamiToolKit.Nodes;

// Custom Implementation of a Node that contains other nodes
public class ListNode() : NodeBase<AtkResNode>(NodeType.Res) {
    private readonly List<NodeBase.NodeBase> nodeList = [];
    
    public required LayoutAnchor LayoutAnchor { get; set; }
    
    /// <summary>
    /// Not implemented yet.
    /// </summary>
    public LayoutOrientation LayoutOrientation { get; set; }

    public void AddNode(NodeBase.NodeBase node) {
        node.AttachNode(this, NodePosition.AsLastChild);
        nodeList.Add(node);

        RecalculateLayout();
    }

    public void RemoveNode(NodeBase.NodeBase node) {
        nodeList.Remove(node);
        node.DetachNode();
        
        RecalculateLayout();
    }
    
    protected override void Dispose(bool isDisposing) {
        if (isDisposing) {
            foreach (var node in nodeList) {
                node.Dispose();
            }
            
            base.Dispose(isDisposing);
        }
    }

    private void RecalculateLayout() {
        var runningPosition = GetLayoutStartPosition();

        foreach (var node in nodeList) {
            if (!node.IsVisible) continue;
            
            switch (LayoutAnchor) {
                case LayoutAnchor.TopLeft: {
                    node.Position = runningPosition + new Vector2(node.Margin.Left, node.Margin.Top);
                    runningPosition.Y += node.Height * node.Scale.Y + node.Margin.Bottom + node.Margin.Top;
                    break;
                }

                case LayoutAnchor.TopRight: {
                    node.Position = runningPosition - new Vector2(node.Margin.Right, 0.0f) + new Vector2(0.0f, node.Margin.Top) - new Vector2(node.Width * node.Scale.X, 0.0f);
                    runningPosition.Y += node.Height * node.Scale.Y + node.Margin.Bottom + node.Margin.Top;
                    break;
                }
                
                case LayoutAnchor.BottomLeft: {
                    node.Position = runningPosition + new Vector2(node.Margin.Left, 0.0f) - new Vector2(0.0f, node.Margin.Bottom) - new Vector2(0.0f, node.Height * node.Scale.Y);
                    runningPosition.Y -= node.Height * node.Scale.Y + node.Margin.Top + node.Margin.Bottom;
                    break;
                }

                case LayoutAnchor.BottomRight: {
                    node.Position = runningPosition - new Vector2(node.Margin.Right, 0.0f) - new Vector2(0.0f, node.Margin.Bottom) - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                    runningPosition.Y -= node.Height * node.Scale.Y + node.Margin.Top + node.Margin.Bottom;

                    break;
                }
            }
        }
    }

    private Vector2 GetLayoutStartPosition() => LayoutAnchor switch {
        LayoutAnchor.TopLeft => Vector2.Zero,
        LayoutAnchor.TopRight => new Vector2(Width, 0.0f),
        LayoutAnchor.BottomLeft => new Vector2(0.0f, Height),
        LayoutAnchor.BottomRight => new Vector2(Width, Height),
        _ => throw new ArgumentOutOfRangeException()
    };
}

public enum LayoutAnchor {
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}

public enum LayoutOrientation {
    Vertical,
    Horizontal,
}