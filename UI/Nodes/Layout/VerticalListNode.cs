using System;
using System.Linq;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A <see cref="LayoutNode"/> that represents a vertical list of elements.
/// </summary>
/// <remarks>
/// If you are needing to represent the same element multiple times use a <see cref="ListNode{T,TU}"/> instead for performance reasons.
/// </remarks>
public class VerticalListNode : LayoutNode, IVerticalListNode {

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
    protected override void OnNavigationUpdate(NavigationContext context) {
        var navTargets = NodeList.OfType<INavigationTarget>()
            .Where(node => node.NavIndex is not 0)
            .ToArray();

        if (navTargets.Length is 0) return;

        for (var i = 0; i < navTargets.Length; i++) {
            var target = navTargets[i];
            target.NavUp = navTargets[(i - 1 + navTargets.Length) % navTargets.Length].NavIndex;
            target.NavDown = navTargets[(i + 1) % navTargets.Length].NavIndex;
        }
    }
}
