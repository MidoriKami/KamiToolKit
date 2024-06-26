using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

// Custom Implementation of a Node that contains other nodes
public class ListNode<T> : NodeBase<AtkResNode>, IList<T> where T : NodeBase {
    private readonly List<T> nodeList = [];
    private readonly ImageNode background;

    private LayoutOrientation InternalLayoutOrientation { get; set; }

    public LayoutAnchor LayoutAnchor { get; set; } = LayoutAnchor.TopLeft;
    
    public LayoutOrientation LayoutOrientation {
        get => InternalLayoutOrientation;
        set {
            InternalLayoutOrientation = value;
            RecalculateLayout();
        } 
    }

    public Vector4 BackgroundColor {
        get => new(background.AddColor.X, background.AddColor.Y, background.AddColor.Z, background.Color.W);
        set {
            background.Color = Vector4.Zero with { W = value.W };
            background.AddColor = value.AsVector3Color();
        }
    }

    public bool BackgroundVisible {
        get => background.IsVisible;
        set => background.IsVisible = value;
    }
    
    public ListNode() : base(NodeType.Res) {
        background = new ImageNode {
            NodeID = 101_000,
            Size = new Vector2(600.0f, 32.0f),
            IsVisible = true,
        };
        
        background.AttachNode(this, NodePosition.AsFirstChild);
    }

    protected override void Dispose(bool isDisposing) {
        if (isDisposing) {
            foreach (var node in nodeList) {
                node.Dispose();
            }
            
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

        background.Size = Size;
    }
    
    /// <summary>
    /// Get the current minimum size that would contain all the nodes including their margins.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMinimumSize() {
        var size = Vector2.Zero;
        
        foreach (var node in nodeList) {
            if (!node.IsVisible) continue;

            switch (LayoutOrientation) {
                // Horizontal we take max height, and add widths
                case LayoutOrientation.Horizontal:
                    size.Y = MathF.Max(size.Y, node.LayoutSize.Y);
                    size.X += node.LayoutSize.X;
                    break;
                
                // Vertical we take max width, and add heights
                case LayoutOrientation.Vertical:
                    size.X = MathF.Max(size.X, node.LayoutSize.X);
                    size.Y += node.LayoutSize.Y;
                    break;
            }
        }

        return size;
    }

    private void CalculateVerticalLayout() {
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
    
        
    private void CalculateHorizontalLayout() {
        var runningPosition = GetLayoutStartPosition();

        foreach (var node in nodeList) {
            if (!node.IsVisible) continue;
            
            switch (LayoutAnchor) {
                case LayoutAnchor.TopLeft: {
                    node.Position = runningPosition + new Vector2(node.Margin.Left, node.Margin.Top);
                    runningPosition.X += node.Width * node.Scale.X + node.Margin.Right + node.Margin.Left;
                    break;
                }

                case LayoutAnchor.TopRight: {
                    node.Position = runningPosition - new Vector2(node.Margin.Right, 0.0f) + new Vector2(0.0f, node.Margin.Top) - new Vector2(node.Width * node.Scale.X, 0.0f);
                    runningPosition.X -= node.Width * node.Scale.X + node.Margin.Left + node.Margin.Right;
                    break;
                }
                
                case LayoutAnchor.BottomLeft: {
                    node.Position = runningPosition + new Vector2(node.Margin.Left, 0.0f) - new Vector2(0.0f, node.Margin.Bottom) - new Vector2(0.0f, node.Height * node.Scale.Y);
                    runningPosition.X += node.Width * node.Scale.X + node.Margin.Left + node.Margin.Right;
                    break;
                }

                case LayoutAnchor.BottomRight: {
                    node.Position = runningPosition - new Vector2(node.Margin.Right, 0.0f) - new Vector2(0.0f, node.Margin.Bottom) - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                    runningPosition.X -= node.Width * node.Scale.X + node.Margin.Left + node.Margin.Right;
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

    public IEnumerator<T> GetEnumerator() 
        => nodeList.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public void Add(T item) {
        item.AttachNode(this, NodePosition.AsLastChild);
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