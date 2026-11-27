using System;
using System.Linq;
using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Primitives.NodeBase;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A list layout node that allows anchoring from any corner, and allows setting a border or background color.
/// </summary>
public class ListBoxNode : LayoutListNode, IListBoxNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorImageNode ColorImageNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public BorderNineGridNode Border { get; }

    /// <inheritdoc />
    public LayoutAnchor LayoutAnchor {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc />
    public bool FitContents {
        get;
        set {
            field = value;
            UpdateLayout();
            Size = GetMinimumSize();
        }
    }

    /// <inheritdoc />
    public LayoutOrientation LayoutOrientation {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc />
    public Vector4 BackgroundColor {
        get => ColorImageNode.Color;
        set => ColorImageNode.Color = value;
    }

    /// <inheritdoc />
    public bool ShowBackground {
        get => ColorImageNode.IsVisible;
        set => ColorImageNode.IsVisible = value;
    }

    /// <inheritdoc />
    public bool ShowBorder {
        get => Border.IsVisible;
        set => Border.IsVisible = value;
    }

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

    /// <summary>
    /// Constructs a new instance of <see cref="ListBoxNode"/>
    /// </summary>
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

    /// <inheritdoc/>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorImageNode.Size = Size;

        Border.Size = Size + new Vector2(30.0f, 30.0f);
        Border.Position = -new Vector2(15.0f, 15.0f);
    }

    /// <inheritdoc/>
    protected override void OnUpdateSizes() { }

    /// <inheritdoc/>
    protected override void OnUpdateLayout() {
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

    /// <inheritdoc/>
    protected override void OnUpdateControllerNav() {
        var navigationGroups = GetNavigationChildGroups();

        if (LayoutOrientation is LayoutOrientation.Horizontal) {
            if (LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
                navigationGroups.Reverse();
            }

            ConnectHorizontalNavigationGroups(navigationGroups);
        }
        else if (LayoutOrientation is LayoutOrientation.Vertical) {
            if (LayoutAnchor is LayoutAnchor.BottomLeft or LayoutAnchor.BottomRight) {
                navigationGroups.Reverse();
            }

            ConnectVerticalNavigationGroups(navigationGroups);
        }

        var boundary = CreateNavigationBoundary(navigationGroups);
        SetNavigationBoundary(boundary);

        if (LayoutOrientation is LayoutOrientation.Horizontal) {
            LinkNavigationBoundaryToIndex(boundary.Left, ControllerNavigationDirection.Left, NavLeft);
            LinkNavigationBoundaryToIndex(boundary.Right, ControllerNavigationDirection.Right, NavRight);

            foreach (var group in navigationGroups) {
                LinkNavigationBoundaryToIndex(group.Boundary.Top, ControllerNavigationDirection.Up, NavUp);
                LinkNavigationBoundaryToIndex(group.Boundary.Bottom, ControllerNavigationDirection.Down, NavDown);
            }
        }
        else if (LayoutOrientation is LayoutOrientation.Vertical) {
            LinkNavigationBoundaryToIndex(boundary.Top, ControllerNavigationDirection.Up, NavUp);
            LinkNavigationBoundaryToIndex(boundary.Bottom, ControllerNavigationDirection.Down, NavDown);

            foreach (var group in navigationGroups) {
                LinkNavigationBoundaryToIndex(group.Boundary.Left, ControllerNavigationDirection.Left, NavLeft);
                LinkNavigationBoundaryToIndex(group.Boundary.Right, ControllerNavigationDirection.Right, NavRight);
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
