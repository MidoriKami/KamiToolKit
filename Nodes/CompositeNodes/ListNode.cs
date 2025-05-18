using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

// Custom Implementation of a Node that contains other nodes
public class ListNode<T> : NodeBase<AtkResNode>, IList<T> where T : NodeBase {
    public readonly ResNode ContainerNode;
    private readonly List<T> nodeList = [];
    public readonly BackgroundImageNode Background;
    public readonly BorderNineGridNode Border;

    private LayoutOrientation InternalLayoutOrientation { get; set; }

    public LayoutAnchor LayoutAnchor { get; set; } = LayoutAnchor.TopLeft;
    
    /// <summary>
    /// If enabled, the background is sized around the content, not the list itself.
    /// </summary>
    public bool BackgroundFitsContents { get; set; }
    public bool BorderFitsContents { get; set; }

    /// <summary>
    /// If enabled, node contents will be clipped inside the container.
    /// </summary>
    public bool ClipListContents {
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

    public ListNode() : base(NodeType.Res) {
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
    
    public LayoutOrientation LayoutOrientation {
        get => InternalLayoutOrientation;
        set {
            InternalLayoutOrientation = value;
            RecalculateLayout();
        } 
    }

    public Vector4 BackgroundColor {
        get => Background.Color;
        set => Background.Color = value;
    }

    public bool BackgroundVisible {
        get => Background.IsVisible;
        set => Background.IsVisible = value;
    }

    public bool BorderVisible {
        get => Border.IsVisible;
        set => Border.IsVisible = value;
    }

    protected override void Dispose(bool isDisposing) {
        if (isDisposing) {
            foreach (var node in nodeList) {
                node.Dispose();
            }
            
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
                Background.Position = topLeftNode.Position;
            }
        }
        else {
            Background.Size = Size;
            Background.Position = Vector2.Zero;
        }
        
        if (BorderFitsContents) {
            Border.Size = GetMinimumSize() + new Vector2(30.0f, 30.0f);

            var topLeftNode = nodeList
                .Where(node => node.IsVisible)
                .MinBy(node => node.Position.Length());
            
            if (nodeList.Count is not 0 && topLeftNode is not null) {
                Border.Position = topLeftNode.Position - new Vector2(15.0f, 15.0f);
            }
        }
        else {
            Border.Size = Size + new Vector2(30.0f, 30.0f);
            Border.Position = - new Vector2(15.0f, 15.0f);
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
        _ => throw new ArgumentOutOfRangeException(),
    };

    public IEnumerator<T> GetEnumerator() 
        => nodeList.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public void Add(T item) {
        item.AttachNode(ContainerNode, NodePosition.AsLastChild);
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
        
        using (var list = ImRaii.TreeNode("List")) {
            if (list) {
                using var table = ImRaii.Table("list_property_table", 2);
                if (table) {
                    ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
                    ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

                    ImGui.TableNextRow();
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Background Color");

                    ImGui.TableNextColumn();
                    var backgroundColor = BackgroundColor;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.ColorEdit4("##BackgroundColor", ref backgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
                        BackgroundColor = backgroundColor;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Text Alignment");
        
                    ImGui.TableNextColumn();
                    var layoutAnchor = LayoutAnchor;
                    if (ComboHelper.EnumCombo("##LayoutAnchor", ref layoutAnchor)) {
                        LayoutAnchor = layoutAnchor;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Layout Orientation");
        
                    ImGui.TableNextColumn();
                    var layoutOrientation = LayoutOrientation;
                    if (ComboHelper.EnumCombo("##LayoutOrientation", ref layoutOrientation)) {
                        LayoutOrientation = layoutOrientation;
                    }
                    
                    ImGui.Spacing();
		
                    ImGui.TableNextColumn();
                    ImGui.Text("Fit Background");
		
                    ImGui.TableNextColumn();
                    var fitContents = BackgroundFitsContents;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.Checkbox("##FitContents", ref fitContents)) {
                        BackgroundFitsContents = fitContents;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Fit Border");
		
                    ImGui.TableNextColumn();
                    var fitBorder = BorderFitsContents;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.Checkbox("##FitBorder", ref fitBorder)) {
                        BorderFitsContents = fitBorder;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Clip List Contents");
		
                    ImGui.TableNextColumn();
                    var clipList = ClipListContents;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.Checkbox("##ClipList", ref clipList)) {
                        ClipListContents = clipList;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Background Visible");
		
                    ImGui.TableNextColumn();
                    var backgroundVisible = BackgroundVisible;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.Checkbox("##BackgroundVisible", ref backgroundVisible)) {
                        BackgroundVisible = backgroundVisible;
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Border Visible");
		
                    ImGui.TableNextColumn();
                    var borderVisible = BorderVisible;
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
                    if (ImGui.Checkbox("##BorderVisible", ref borderVisible)) {
                        BorderVisible = borderVisible;
                    }
                }
            }
        }
        
        using (var container = ImRaii.TreeNode("Container")) {
            if (container) {
                ContainerNode.DrawConfig();
            }
        }

        using (var background = ImRaii.TreeNode("Background")) {
            if (background) {
                Background.DrawConfig();
            }
        }

        using (var border = ImRaii.TreeNode("Border")) {
            if (border) {
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