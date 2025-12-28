using System.Linq;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class VerticalListNode : LayoutListNode {

    public VerticalListAnchor Alignment {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// Resizes this layout node to fit the height of the contained nodes.
    /// </summary>
    public bool FitContents { get; set; }

    /// <summary>
    /// Resizes nodes that are inserted to be the same width as the content area
    /// </summary>
    public bool FitWidth { get; set; }

    protected override void InternalRecalculateLayout() {
        var startY = Alignment switch {
            VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
            VerticalListAnchor.Bottom => Height,
            _ => 0.0f,
        };

        foreach (var node in NodeList) {
            if (!node.IsVisible) continue;

            if (Alignment is VerticalListAnchor.Bottom) {
                startY -= node.Height + ItemSpacing;
            }

            node.Y = startY;

            if (FitWidth) {
                node.Width = Width;
            }

            AdjustNode(node);

            if (Alignment is VerticalListAnchor.Top) {
                startY += node.Height + ItemSpacing;
            }
        }

        if (FitContents) {
            Height = NodeList.Sum(node => node.IsVisible ? node.Height + ItemSpacing : 0.0f) + FirstItemSpacing;
        }
    }
}
