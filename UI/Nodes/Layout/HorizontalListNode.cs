using System.Linq;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A <see cref="LayoutNode"/> that represents a horizontally growing list of nodes.
/// Can be anchored on left side or right side.
/// </summary>
public class HorizontalListNode : LayoutNode, IHorizontalListNode {

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

            if (Alignment is HorizontalListAnchor.Left) {
                startX += node.Width + ItemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnNavigationUpdate(NavigationContext context) {
        // todo: this as well
    }
}
