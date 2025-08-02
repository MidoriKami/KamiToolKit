using System;
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
            NodeId = 2, IsVisible = true,
        };
        Background.AttachNode(this);

        Border = new BorderNineGridNode {
            NodeId = 3, Position = new Vector2(-15.0f, -15.0f), IsVisible = false,
        };
        Border.AttachNode(this);
    }

    protected override uint ListBaseId => 3;

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

    [JsonProperty] public Spacing ItemMargin {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    } = new(0.0f);

    public override float Height {
        get;
        set {
            field = FitContents ? GetMinimumSize().Y : value;

            base.Height = field;

            Background.Height = field;
            Border.Height = field + 30.0f;

            RecalculateLayout();
        }
    }

    public override float Width {
        get;
        set {
            field = FitContents ? GetMinimumSize().X : value;

            base.Width = field;

            Background.Width = field;
            Border.Width = field + 30.0f;

            RecalculateLayout();
        }
    }

    public override void RecalculateLayout() {
        switch (LayoutOrientation) {
            case LayoutOrientation.Vertical:
                CalculateVerticalLayout();
                break;

            case LayoutOrientation.Horizontal:
                CalculateHorizontalLayout();
                break;
        }
    }

    /// <summary>
    ///     Get the current minimum size that would contain all the nodes including their margins.
    /// </summary>
    public Vector2 GetMinimumSize() {
        var size = Vector2.Zero;

        foreach (var node in NodeList) {
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

        if (LayoutAnchor is LayoutAnchor.TopLeft or LayoutAnchor.TopRight) {
            runningPosition += new Vector2(0.0f, FirstItemSpacing);
        }
        else if (LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.BottomRight) {
            runningPosition -= new Vector2(0.0f, FirstItemSpacing);
        }

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            var netMargin = node.Margin + ItemMargin + new Spacing(ItemSpacing / 2.0f, 0.0f, 0.0f, ItemSpacing / 2.0f);

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

        if (LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.TopLeft) {
            runningPosition += new Vector2(FirstItemSpacing, 0.0f);
        }
        else if (LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
            runningPosition -= new Vector2(FirstItemSpacing, 0.0f);
        }

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            var netMargin = node.Margin + ItemMargin + new Spacing(0.0f, ItemSpacing / 2.0f, ItemSpacing / 2.0f, 0.0f);

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
