using System.Linq;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

/// <summary>
/// A <see cref="LayoutListNode"/> that represents a horizontally growing list of nodes.
/// Can be anchored on left side or right side.
/// </summary>
public class HorizontalListNode : LayoutListNode {

    /// <summary>
    /// Gets or sets the alignment used when calculating layout.
    /// </summary>
    /// <remarks>
    /// Setting triggers layout recalculation.
    /// </remarks>
    public HorizontalListAnchor Alignment {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <inheritdoc/>
    public override float Width {
        get => base.Width;
        set {
            base.Width = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// Adjusts contained nodes heights to match this nodes height
    /// </summary>
    public bool FitHeight { get; set; }

    /// <summary>
    /// Resizes the horizontal list node to fit all contents
    /// </summary>
    public bool FitToContentHeight { get; set; }

    /// <summary>
    /// Gets the amount of space remaining in this node.
    /// </summary>
    public float AreaRemaining
        => Width - NodeList.Sum(node => node.Width + ItemSpacing) - ItemSpacing;

    /// <summary>
    /// Gets or sets the up nav index.
    /// </summary>
    public int NavUp { get; set; }

    /// <summary>
    /// Gets or sets the down nav index.
    /// </summary>
    public int NavDown { get; set; }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
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

            if (FitHeight) {
                node.Height = Height;
            }
        }

        if (FitToContentHeight) {
            Height = NodeList.Max(node => node.Height);
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
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
