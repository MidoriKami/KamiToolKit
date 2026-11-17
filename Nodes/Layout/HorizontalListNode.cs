using System.Linq;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A <see cref="LayoutListNode"/> that represents a horizontally growing list of nodes.
/// Can be anchored on left side or right side.
/// </summary>
public class HorizontalListNode : LayoutListNode, IHorizontalListNode {

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Horizontal;

    /// <inheritdoc/>
    public HorizontalListAnchor Alignment {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public override float Width {
        get => base.Width;
        set {
            base.Width = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public bool FitHeight { get; set; }

    /// <inheritdoc/>
    public bool FitToContentHeight { get; set; }

    /// <inheritdoc/>
    public bool FitToContentWidth { get; set; }

    /// <inheritdoc/>
    public float AreaRemaining
        => Width - NodeList.Sum(node => node.Width + ItemSpacing) - ItemSpacing;

    /// <inheritdoc />
    protected override void OnUpdateSizes() {
        if (FitToContentWidth) {
            base.Width = NodeList.Sum(node => node.IsVisible ? node.Width + ItemSpacing : 0.0f) + FirstItemSpacing - ItemSpacing;
        }

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (FitHeight) {
                node.Height = Height;
            }
        }

        if (FitToContentHeight) {
            Height = NodeList.Max(node => node.Height);
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        var startX = Alignment switch {
            HorizontalListAnchor.Left => 0.0f + FirstItemSpacing,
            HorizontalListAnchor.Right => Width - FirstItemSpacing,
            _ => 0.0f,
        };

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (Alignment is HorizontalListAnchor.Right) {
                startX -= node.Width + ItemSpacing;
            }

            node.X = startX;
            AdjustNode(node);

            if (Alignment is HorizontalListAnchor.Left) {
                startX += node.Width + ItemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
        var componentNodes = NodeList.OfType<ComponentNode>().ToList();
        if (componentNodes.Count is 0) return;

        if (Alignment is HorizontalListAnchor.Right) {
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
}
