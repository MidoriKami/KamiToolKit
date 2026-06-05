using System;
using System.Linq;
using System.Numerics;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A list layout node that allows anchoring from any corner, and allows setting a border or background color.
/// </summary>
public class ListBoxNode : LayoutListNode, IControllerNavigable {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorImageNode ColorImageNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public BorderNineGridNode Border { get; }

    /// <summary>
    /// Get or sets the layout anchor position.
    /// </summary>
    /// <remarks>
    /// When set recalculates node layout.
    /// </remarks>
    public LayoutAnchor LayoutAnchor {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// Gets or sets whether the node should resize to fit its contents.
    /// </summary>
    public bool FitContents {
        get;
        set {
            field = value;
            RecalculateLayout();
            Size = GetMinimumSize();
        }
    }

    /// <summary>
    /// Gets or sets which direction nodes will travel relative to the <see cref="LayoutAnchor"/>.
    /// </summary>
    public LayoutOrientation LayoutOrientation {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Vector4 BackgroundColor {
        get => ColorImageNode.Color;
        set => ColorImageNode.Color = value;
    }

    /// <summary>
    /// Gets or sets whether the background color is shown.
    /// </summary>
    public bool ShowBackground {
        get => ColorImageNode.IsVisible;
        set => ColorImageNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets whether the border is shown.
    /// </summary>
    public bool ShowBorder {
        get => Border.IsVisible;
        set => Border.IsVisible = value;
    }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

    /// <inheritdoc/>
    public override float Height {
        get => base.Height;
        set => base.Height = FitContents ? GetMinimumSize().Y : value;
    }

    /// <inheritdoc/>
    public override float Width {
        get => base.Width;
        set => base.Width = FitContents ? GetMinimumSize().X : value;
    }

    /// <inheritdoc/>
    public override void AddNode(NodeBase? node) {
        base.AddNode(node);

        if (FitContents) {
            Size = GetMinimumSize();
        }
    }

    /// <inheritdoc/>
    public override void RemoveNode(NodeBase node) {
        base.RemoveNode(node);

        if (FitContents) {
            Size = GetMinimumSize();
        }
    }

    /// <summary>
    /// Get the current minimum size that would contain all the nodes including their margins.
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

    public ListBoxNode() {
        ColorImageNode = new ColorImageNode {
            IsVisible = false,
        };
        ColorImageNode.AttachNode(this);

        Border = new BorderNineGridNode {
            IsVisible = false,
        };
        Border.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorImageNode.Size = Size;

        Border.Size = Size + new Vector2(30.0f, 30.0f);
        Border.Position = -new Vector2(15.0f, 15.0f);

        RecalculateLayout();
    }

    protected override void OnRecalculateLayout() {
        var runningPosition = LayoutOrientation switch {
            LayoutOrientation.Vertical when LayoutAnchor is LayoutAnchor.TopLeft or LayoutAnchor.TopRight
                => GetLayoutStartPosition() + new Vector2(0.0f, FirstItemSpacing),

            LayoutOrientation.Vertical when LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.BottomRight
                => GetLayoutStartPosition() - new Vector2(0.0f, FirstItemSpacing),

            LayoutOrientation.Horizontal when LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.TopLeft
                => GetLayoutStartPosition() + new Vector2(FirstItemSpacing, 0.0f),

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

    protected override void OnRecalculateNavigation() {
        var componentNodes = NodeList.OfType<ComponentNode>().ToList();
        if (componentNodes.Count is 0) return;

        if (LayoutOrientation is LayoutOrientation.Horizontal) {
            if (LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
                componentNodes = componentNodes.AsEnumerable().Reverse().ToList();
            }

            foreach (var (index, node) in componentNodes.Index()) {
                node.NavIndex = index + NavIndex;
                node.NavUp = NavUp;
                node.NavDown = NavDown;

                // First Element
                if (index is 0) {
                    node.NavLeft = componentNodes.Count - 1 + NavIndex;
                }
                else {
                    node.NavLeft = index - 1 + NavIndex;
                }

                // Last Element
                if (index == componentNodes.Count - 1) {
                    node.NavRight = NavIndex;
                }
                else {
                    node.NavRight = index + 1 + NavIndex;
                }
            }
        }
        else if (LayoutOrientation is LayoutOrientation.Vertical) {
            if (LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.BottomRight) {
                componentNodes = componentNodes.AsEnumerable().Reverse().ToList();
            }

            foreach (var (index, node) in componentNodes.Index()) {
                node.NavIndex = (byte) (index + NavIndex);
                node.NavLeft = NavLeft;
                node.NavRight = NavRight;

                // First Element
                if (index is 0) {
                    node.NavUp = (byte) (componentNodes.Count - 1 + NavIndex);
                }
                else {
                    node.NavUp = (byte) (index - 1 + NavIndex);
                }

                // Last Element
                if (index == componentNodes.Count - 1) {
                    node.NavDown = (byte) NavIndex;
                }
                else {
                    node.NavDown = (byte) (index + 1 + NavIndex);
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
}
