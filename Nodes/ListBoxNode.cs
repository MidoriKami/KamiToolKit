using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// Custom Implementation of a Node that contains other nodes
[JsonObject(MemberSerialization.OptIn)]
public unsafe class ListBoxNode<T> : NodeBase<AtkResNode>, IList<T> where T : NodeBase {
    [JsonProperty] protected readonly ResNode ContainerNode;
    private readonly List<T> nodeList = [];
    [JsonProperty] protected readonly BackgroundImageNode Background;
    [JsonProperty] protected readonly BorderNineGridNode Border;

    private LayoutOrientation InternalLayoutOrientation { get; set; }
    private LayoutAnchor InternalLayoutAnchor { get; set; } = LayoutAnchor.TopLeft;
    private bool InternalBackgroundFitContents { get; set; }
    private bool InternalBorderFitContents { get; set; }

    [JsonProperty] public LayoutAnchor LayoutAnchor {
        get => InternalLayoutAnchor;
        set {
            InternalLayoutAnchor = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// If enabled, the background is sized around the content, not the list itself.
    /// </summary>
    [JsonProperty] public bool BackgroundFitsContents {
        get => InternalBackgroundFitContents;
        set {
            InternalBackgroundFitContents = value;
            RecalculateLayout();
        }
    }

    [JsonProperty] public bool BorderFitsContents {
        get => InternalBorderFitContents;
        set {
            InternalBorderFitContents = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// If enabled, node contents will be clipped inside the container.
    /// </summary>
    [JsonProperty] public bool ClipListContents {
        get => ContainerNode.NodeFlags.HasFlag(NodeFlags.Clip);
        set {
            if (value) {
                ContainerNode.AddFlags(NodeFlags.Clip);
            }
            else {
                ContainerNode.RemoveFlags(NodeFlags.Clip);
            }
        }
    }

    public ListBoxNode() : base(NodeType.Res) {
        ContainerNode = new ResNode {
            NodeId = 103_000,
            Size = new Vector2(600.0f, 32.0f),
            IsVisible = true,
        };
        
        ContainerNode.AttachNode(this, NodePosition.AsFirstChild);
        
        Border = new BorderNineGridNode {
            NodeId = 102_000,
            Size = new Vector2(600.0f, 32.0f),
            Position = new Vector2(-15.0f, -15.0f),
            IsVisible = false,
        };
        
        Border.AttachNode(this, NodePosition.AsFirstChild);
        
        Background = new BackgroundImageNode {
            NodeId = 101_000,
            Size = new Vector2(600.0f, 32.0f),
            IsVisible = true,
        };
        
        Background.AttachNode(this, NodePosition.AsFirstChild);
    }
    
    [JsonProperty] public LayoutOrientation LayoutOrientation {
        get => InternalLayoutOrientation;
        set {
            InternalLayoutOrientation = value;
            RecalculateLayout();
        } 
    }

    [JsonProperty] public Vector4 BackgroundColor {
        get => Background.Color;
        set => Background.Color = value;
    }

    [JsonProperty] public bool BackgroundVisible {
        get => Background.IsVisible;
        set => Background.IsVisible = value;
    }

    [JsonProperty] public bool BorderVisible {
        get => Border.IsVisible;
        set => Border.IsVisible = value;
    }

    public new float Width {
        get => InternalResNode->GetWidth();
        set {
            InternalResNode->SetWidth((ushort) value);
            RecalculateLayout();
        }
    }

    public new float Height {
        get => InternalResNode->GetHeight();
        set {
            InternalResNode->SetHeight((ushort) value);
            RecalculateLayout();
        }
    }

    [JsonProperty] public new Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    private Spacing InternalSpacing { get; set; } = new(0.0f);

    [JsonProperty] public Spacing ItemMargin {
        get => InternalSpacing;
        set {
            InternalSpacing = value;
            RecalculateLayout();
        }
    }

    protected override void Dispose(bool isDisposing) {
        if (isDisposing) {
            foreach (var node in nodeList) {
                node.Dispose();
            }
            
            ContainerNode.Dispose();
            Background.Dispose();
            Border.Dispose();
            
            base.Dispose(isDisposing);
        }
    }

    public void RecalculateLayout() {
        switch (LayoutOrientation) {
            case LayoutOrientation.Vertical:
                CalculateVerticalLayout();
                break;
            
            case LayoutOrientation.Horizontal:
                CalculateHorizontalLayout();
                break;
        }

        if (BackgroundFitsContents) {
            Background.Size = GetMinimumSize();

            var topLeftNode = nodeList
                .Where(node => node.IsVisible)
                .MinBy(node => node.Position.Length());
            
            if (nodeList.Count is not 0 && topLeftNode is not null) {
                Background.Position = topLeftNode.Position - new Vector2(ItemMargin.Left, ItemMargin.Top);
            }
        }
        else {
            Background.Size = Size;
            Background.Position = Vector2.Zero - new Vector2(ItemMargin.Left, ItemMargin.Top);
        }
        
        if (BorderFitsContents) {
            Border.Size = GetMinimumSize() + new Vector2(30.0f, 30.0f);

            var topLeftNode = nodeList
                .Where(node => node.IsVisible)
                .MinBy(node => node.Position.Length());
            
            if (nodeList.Count is not 0 && topLeftNode is not null) {
                Border.Position = topLeftNode.Position - new Vector2(15.0f, 15.0f) - new Vector2(ItemMargin.Left, ItemMargin.Top);
            }
        }
        else {
            Border.Size = Size + new Vector2(30.0f, 30.0f);
            Border.Position = - new Vector2(15.0f, 15.0f) - new Vector2(ItemMargin.Left, ItemMargin.Top);
        }
        
        ContainerNode.Size = Size;
    }
    
    /// <summary>
    /// Get the current minimum size that would contain all the nodes including their margins.
    /// </summary>
    public Vector2 GetMinimumSize() {
        var size = Vector2.Zero;
        
        foreach (var node in nodeList) {
            if (!node.IsVisible) continue;

            switch (LayoutOrientation) {
                // Horizontal we take max height, and add widths
                case LayoutOrientation.Horizontal:
                    size.Y = MathF.Max(size.Y, node.LayoutSize.Y + ItemMargin.Top + ItemMargin.Bottom);
                    size.X += node.LayoutSize.X + ItemMargin.Right + ItemMargin.Left;
                    break;
                
                // Vertical we take max width, and add heights
                case LayoutOrientation.Vertical:
                    size.X = MathF.Max(size.X, node.LayoutSize.X + ItemMargin.Left + ItemMargin.Right);
                    size.Y += node.LayoutSize.Y + ItemMargin.Top + ItemMargin.Bottom;
                    break;
            }
        }

        return size;
    }

    private void CalculateVerticalLayout() {
        var runningPosition = GetLayoutStartPosition();
        
        foreach (var node in nodeList) {
            if (!node.IsVisible) {
                if (node.NodeFlags.HasFlag(NodeFlags.HasCollision)) {
                    node.RemoveFlags(NodeFlags.HasCollision);
                }
                continue;
            }
            else {
                if (!node.NodeFlags.HasFlag(NodeFlags.HasCollision)) {
                    node.AddFlags(NodeFlags.HasCollision);
                }
            }
            
            var netMargin = node.Margin + ItemMargin;
            
            switch (LayoutAnchor) {
                case LayoutAnchor.TopLeft: {
                    node.Position = runningPosition + new Vector2(netMargin.Left, netMargin.Top);
                    runningPosition.Y += node.Height * node.Scale.Y + netMargin.Bottom + netMargin.Top;
                    break;
                }

                case LayoutAnchor.TopRight: {
                    node.Position = runningPosition - new Vector2(netMargin.Right, 0.0f) + new Vector2(0.0f, netMargin.Top) - new Vector2(node.Width * node.Scale.X, 0.0f);
                    runningPosition.Y += node.Height * node.Scale.Y + netMargin.Bottom + netMargin.Top;
                    break;
                }
                
                case LayoutAnchor.BottomLeft: {
                    node.Position = runningPosition + new Vector2(netMargin.Left, 0.0f) - new Vector2(0.0f, netMargin.Bottom) - new Vector2(0.0f, node.Height * node.Scale.Y);
                    runningPosition.Y -= node.Height * node.Scale.Y + netMargin.Top + netMargin.Bottom;
                    break;
                }

                case LayoutAnchor.BottomRight: {
                    node.Position = runningPosition - new Vector2(netMargin.Right, 0.0f) - new Vector2(0.0f, netMargin.Bottom) - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                    runningPosition.Y -= node.Height * node.Scale.Y + netMargin.Top + netMargin.Bottom;
                    break;
                }
            }
        }
    }
        
    private void CalculateHorizontalLayout() {
        var runningPosition = GetLayoutStartPosition();

        foreach (var node in nodeList) {
            if (!node.IsVisible) continue;
            
            var netMargin = node.Margin + ItemMargin;
            
            switch (LayoutAnchor) {
                case LayoutAnchor.TopLeft: {
                    node.Position = runningPosition + new Vector2(netMargin.Left, netMargin.Top);
                    runningPosition.X += node.Width * node.Scale.X + netMargin.Right + netMargin.Left;
                    break;
                }

                case LayoutAnchor.TopRight: {
                    node.Position = runningPosition - new Vector2(netMargin.Right, 0.0f) + new Vector2(0.0f, netMargin.Top) - new Vector2(node.Width * node.Scale.X, 0.0f);
                    runningPosition.X -= node.Width * node.Scale.X + netMargin.Left + netMargin.Right;
                    break;
                }
                
                case LayoutAnchor.BottomLeft: {
                    node.Position = runningPosition + new Vector2(netMargin.Left, 0.0f) - new Vector2(0.0f, netMargin.Bottom) - new Vector2(0.0f, node.Height * node.Scale.Y);
                    runningPosition.X += node.Width * node.Scale.X + netMargin.Left + netMargin.Right;
                    break;
                }

                case LayoutAnchor.BottomRight: {
                    node.Position = runningPosition - new Vector2(netMargin.Right, 0.0f) - new Vector2(0.0f, netMargin.Bottom) - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                    runningPosition.X -= node.Width * node.Scale.X + netMargin.Left + netMargin.Right;
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
        _ => throw new ArgumentOutOfRangeException(),
    };

    public IEnumerator<T> GetEnumerator() 
        => nodeList.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public void Add(T item) {
        item.AttachNode(ContainerNode);
        nodeList.Add(item);
        
        RecalculateLayout();
    }
    
    public void Clear() {
        foreach (var node in nodeList) {
            node.DetachNode();
            node.Dispose();
        }
        
        nodeList.Clear();
        RecalculateLayout();
    }
    
    public bool Contains(T item) 
        => nodeList.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
        => nodeList.CopyTo(array, arrayIndex);
    
    public bool Remove(T item) {
        item.DetachNode();
        nodeList.Remove(item);
        item.Dispose();
        
        RecalculateLayout();
        
        return true;
    }

    public int Count => nodeList.Count;

    public bool IsReadOnly => false;

    public int IndexOf(T item) 
        => nodeList.IndexOf(item);

    public void Insert(int index, T item) 
        => nodeList.Insert(index, item);

    public void RemoveAt(int index) 
        => nodeList.RemoveAt(index);

    public T this[int index] {
        get => nodeList[index];
        set => nodeList[index] = value;
    }

    public override void DrawConfig() {
        base.DrawConfig();
        
        using (var container = ImRaii.TreeNode("Container")) {
            if (container) {
                ContainerNode.DrawConfig();
            }
        }
    
        using (var backgroundNode = ImRaii.TreeNode("Background")) {
            if (backgroundNode) {
                Background.DrawConfig();
            }
        }
    
        using (var borderNode = ImRaii.TreeNode("Border")) {
            if (borderNode) {
                Border.DrawConfig();
            }
        }
    }
}

public enum LayoutAnchor {
    [Description("Top Left")]
    TopLeft,
    
    [Description("Top Right")]
    TopRight,
    
    [Description("Bottom Left")]
    BottomLeft,
    
    [Description("Bottom Right")]
    BottomRight,
}

public enum LayoutOrientation {
    Vertical,
    Horizontal,
}