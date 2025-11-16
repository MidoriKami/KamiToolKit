using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// Node that manages the layout of other nodes
[JsonObject(MemberSerialization.OptIn)]
public class ListBoxNode : LayoutListNode {

    [JsonProperty] public readonly BackgroundImageNode Background;
    [JsonProperty] public readonly BorderNineGridNode Border;

    public ListBoxNode() {
        Background = new BackgroundImageNode {
            IsVisible = false,
        };
        Background.AttachNode(this);

        Border = new BorderNineGridNode {
            IsVisible = false,
        };
        Border.AttachNode(this);
    }

    [JsonProperty] public LayoutAnchor LayoutAnchor {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    [JsonProperty] public bool FitContents {
        get;
        set {
            field = value;
            RecalculateLayout();
            Size = GetMinimumSize();
        }
    }

    [JsonProperty] public LayoutOrientation LayoutOrientation {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    [JsonProperty] public Vector4 BackgroundColor {
        get => Background.Color;
        set => Background.Color = value;
    }

    [JsonProperty] public bool ShowBackground {
        get => Background.IsVisible;
        set => Background.IsVisible = value;
    }

    [JsonProperty] public bool ShowBorder {
        get => Border.IsVisible;
        set => Border.IsVisible = value;
    }

    public override float Height {
        get => base.Height;
        set => base.Height = FitContents ? GetMinimumSize().Y : value;
    }

    public override float Width {
        get => base.Width;
        set => base.Width = FitContents ? GetMinimumSize().X : value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        Background.Size = Size;

        Border.Size = Size + new Vector2(30.0f, 30.0f);
        Border.Position = -new Vector2(15.0f, 15.0f);
        
        RecalculateLayout();
    }

    protected override void InternalRecalculateLayout() {
        var runningPosition = LayoutOrientation switch {
            LayoutOrientation.Vertical when LayoutAnchor is LayoutAnchor.TopLeft or LayoutAnchor.TopRight 
                => GetLayoutStartPosition() + new Vector2(0.0f, FirstItemSpacing),

            LayoutOrientation.Vertical when LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.BottomRight 
                => GetLayoutStartPosition() - new Vector2(0.0f, FirstItemSpacing),

            LayoutOrientation.Horizontal when LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.TopLeft 
                =>  GetLayoutStartPosition() + new Vector2(FirstItemSpacing, 0.0f),

            LayoutOrientation.Horizontal when LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight
                => GetLayoutStartPosition() - new Vector2(FirstItemSpacing, 0.0f),

            _ => Vector2.Zero,
        };

        foreach (var node in NodeList.Where(node => node.IsVisible)) {
            if (LayoutOrientation is LayoutOrientation.Vertical) {
                switch (LayoutAnchor) {
                    case LayoutAnchor.TopLeft: 
                        node.Position = runningPosition;
                        runningPosition.Y += node.Height * node.Scale.Y + ItemSpacing;
                        break;
            
                    case LayoutAnchor.TopRight: 
                        node.Position = runningPosition - new Vector2(node.Width * node.Scale.X, 0.0f);
                        runningPosition.Y += node.Height * node.Scale.Y + ItemSpacing;
                        break;
            
                    case LayoutAnchor.BottomLeft:
                        node.Position = runningPosition - new Vector2(0.0f, node.Height * node.Scale.Y);
                        runningPosition.Y -= node.Height * node.Scale.Y + ItemSpacing;
                        break;
            
                    case LayoutAnchor.BottomRight:
                        node.Position = runningPosition - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                        runningPosition.Y -= node.Height * node.Scale.Y + ItemSpacing;
                        break;
                }
            }
            else if (LayoutOrientation is LayoutOrientation.Horizontal) {
                switch (LayoutAnchor) {
                    case LayoutAnchor.TopLeft:
                        node.Position = runningPosition;
                        runningPosition.X += node.Width * node.Scale.X + ItemSpacing;
                        break;
            
                    case LayoutAnchor.TopRight:
                        node.Position = runningPosition - new Vector2(node.Width * node.Scale.X, 0.0f);
                        runningPosition.X -= node.Width * node.Scale.X + ItemSpacing;
                        break;
                
                    case LayoutAnchor.BottomLeft:
                        node.Position = runningPosition - new Vector2(0.0f, node.Height * node.Scale.Y);
                        runningPosition.X += node.Width * node.Scale.X + ItemSpacing;
                        break;
                
                    case LayoutAnchor.BottomRight:
                        node.Position = runningPosition - new Vector2(node.Width * node.Scale.X, node.Height * node.Scale.Y);
                        runningPosition.X -= node.Width * node.Scale.X + ItemSpacing;
                        break;
                }
            }
        }
    }

    public override void AddNode(NodeBase node, bool suppressRecalculateLayout = false) {
        base.AddNode(node, suppressRecalculateLayout);
        Size = GetMinimumSize();
    }

    public override void RemoveNode(NodeBase node, bool suppressRecalculateLayout = false) {
        base.RemoveNode(node, suppressRecalculateLayout);
        Size = GetMinimumSize();
    }

    /// <summary>
    ///     Get the current minimum size that would contain all the nodes including their margins.
    /// </summary>
    public Vector2 GetMinimumSize() {
        var size = LayoutOrientation switch {
            LayoutOrientation.Vertical => new Vector2(0.0f, FirstItemSpacing),
            LayoutOrientation.Horizontal => new Vector2(FirstItemSpacing, 0.0f),
            _ => Vector2.Zero,
        };

        foreach (var node in NodeList.Where(node => node.IsVisible)) {
            switch (LayoutOrientation) {
                // Horizontal we take max height, and add widths
                case LayoutOrientation.Horizontal:
                    size.Y = MathF.Max(size.Y, node.Height);
                    size.X += node.Width + ItemSpacing;
                    break;

                // Vertical we take max width, and add heights
                case LayoutOrientation.Vertical:
                    size.X = MathF.Max(size.X, node.Width);
                    size.Y += node.Height + ItemSpacing;
                    break;
            }
        }

        return size;
    }

    private Vector2 GetLayoutStartPosition() => LayoutAnchor switch {
        LayoutAnchor.TopLeft => Vector2.Zero,
        LayoutAnchor.TopRight => new Vector2(Width, 0.0f),
        LayoutAnchor.BottomLeft => new Vector2(0.0f, Height),
        LayoutAnchor.BottomRight => new Vector2(Width, Height),
        _ => throw new ArgumentOutOfRangeException(),
    };

    public override void DrawConfig() {
        base.DrawConfig();

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
