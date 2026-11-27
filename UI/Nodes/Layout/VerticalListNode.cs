using System;
using System.Linq;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Interfaces.Nodes;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A <see cref="LayoutListNode"/> that represents a vertical list of elements.
/// </summary>
/// <remarks>
/// If you are needing to represent the same element multiple times use a <see cref="ListNode{T,TU}"/> instead for performance reasons.
/// </remarks>
public class VerticalListNode : LayoutListNode, IVerticalListNode {

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Vertical;

    /// <inheritdoc/>
    public VerticalListAnchor Anchor {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public VerticalListAlignment Alignment {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public bool FitContents { get; set; }

    /// <inheritdoc/>
    public bool StretchWidth { get; set; }

    /// <inheritdoc />
    protected override void OnUpdateSizes() {
        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (StretchWidth) {
                node.Width = Width;
            }
        }

        if (FitContents) {
            Height = Math.Max(0.0f, NodeList.Sum(node => node.IsVisible ? node.Height + ItemSpacing : 0.0f) + FirstItemSpacing - ItemSpacing);
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        var startY = Anchor switch {
            VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
            VerticalListAnchor.Bottom => Height,
            _ => 0.0f,
        };

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (Anchor is VerticalListAnchor.Bottom) {
                startY -= node.Height + ItemSpacing;
            }

            node.Y = startY;

            if (!StretchWidth) {
                switch (Alignment) {
                    case VerticalListAlignment.Right:
                        node.X = Width - node.Width;
                        break;

                    case VerticalListAlignment.Left:
                        node.X = 0.0f;
                        break;
                }
            }

            if (Anchor is VerticalListAnchor.Top) {
                startY += node.Height + ItemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
        var navigationGroups = GetNavigationChildGroups();

        if (Anchor is VerticalListAnchor.Bottom) {
            navigationGroups.Reverse();
        }

        ConnectVerticalNavigationGroups(navigationGroups);

        var boundary = CreateNavigationBoundary(navigationGroups);
        SetNavigationBoundary(boundary);
        LinkNavigationBoundaryToIndex(boundary.Top, ControllerNavigationDirection.Up, NavUp);
        LinkNavigationBoundaryToIndex(boundary.Bottom, ControllerNavigationDirection.Down, NavDown);

        foreach (var group in navigationGroups) {
            LinkNavigationBoundaryToIndex(group.Boundary.Left, ControllerNavigationDirection.Left, NavLeft);
            LinkNavigationBoundaryToIndex(group.Boundary.Right, ControllerNavigationDirection.Right, NavRight);
        }
    }
}
