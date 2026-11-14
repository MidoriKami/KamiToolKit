using System.Linq;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

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
            RecalculateLayout();
        }
    }

    /// <inheritdoc/>
    public VerticalListAlignment Alignment {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <inheritdoc/>
    public bool FitContents { get; set; }

    /// <inheritdoc/>
    public bool StretchWidth { get; set; }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
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

            if (StretchWidth) {
                node.Width = Width;
            }
            else {
                switch (Alignment) {
                    case VerticalListAlignment.Right:
                        node.X = Width - node.Width;
                        break;

                    case VerticalListAlignment.Left:
                        node.X = 0.0f;
                        break;
                }
            }

            AdjustNode(node);

            if (Anchor is VerticalListAnchor.Top) {
                startY += node.Height + ItemSpacing;
            }
        }

        if (FitContents) {
            Height = NodeList.Sum(node => node.IsVisible ? node.Height + ItemSpacing : 0.0f) + FirstItemSpacing - ItemSpacing;
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        var componentNodes = NodeList.OfType<ComponentNode>().ToList();
        if (componentNodes.Count is 0) return;

        if (Anchor is VerticalListAnchor.Bottom) {
            componentNodes = [
                .. componentNodes.AsEnumerable().Reverse(),
            ];
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
