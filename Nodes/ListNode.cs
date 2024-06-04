using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit;

// Custom Implementation of a Node that contains other nodes
public unsafe class ListNode() : NodeBase<AtkResNode>(NodeType.Res) {
    private List<NodeBase> nodeList = [];
    
    public required LayoutAnchor LayoutAnchor { get; set; }

    public void AddNode(NodeBase node) {
        var parentAddon = AddonLocator.GetAddonForNode(InternalResNode);
        if (parentAddon is null) return;
        
        NodeLinker.AttachNode(parentAddon, node.InternalResNode, InternalResNode, NodePosition.AsLastChild);
        nodeList.Add(node);
        
        RecalculateLayout();
    }
    
    protected override void Dispose(bool isDisposing) {
        if (isDisposing) {
            
            base.Dispose(isDisposing);
        }
    }

    private void RecalculateLayout() {
        var runningPosition = GetLayoutStartPosition();

        foreach (var node in nodeList) {
            switch (LayoutAnchor) {
                case LayoutAnchor.TopLeft: {
                    node.Position = runningPosition + new Vector2(node.Padding.Left, node.Padding.Top);
                    runningPosition.Y += node.Height + node.Padding.Bottom + node.Padding.Top;
                    break;
                }

                case LayoutAnchor.TopRight: {
                    node.Position = runningPosition - new Vector2(node.Padding.Right, 0.0f) + new Vector2(0.0f, node.Padding.Top) - new Vector2(node.Width, 0.0f);
                    runningPosition.Y += node.Height + node.Padding.Bottom + node.Padding.Top;
                    break;
                }
                
                case LayoutAnchor.BottomLeft: {
                    node.Position = runningPosition + new Vector2(node.Padding.Left, 0.0f) - new Vector2(0.0f, node.Padding.Bottom) - new Vector2(0.0f, node.Height);
                    runningPosition.Y -= node.Height + node.Padding.Top + node.Padding.Bottom;
                    break;
                }

                case LayoutAnchor.BottomRight: {
                    node.Position = runningPosition - new Vector2(node.Padding.Right, 0.0f) - new Vector2(0.0f, node.Padding.Bottom) - new Vector2(node.Width, node.Height);
                    runningPosition.Y -= node.Height + node.Padding.Top + node.Padding.Bottom;

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