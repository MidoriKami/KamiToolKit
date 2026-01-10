using System.Linq;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

public class VerticalListNode : LayoutListNode {

    /// <summary>
    /// Displays items starting from either the bottom or the top of the list
    /// </summary>
    public VerticalListAnchor Anchor {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    /// <summary>
    /// Displays items either left aligned or right aligned
    /// </summary>
    public VerticalListAlignment Alignment {
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

            if (FitWidth) {
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
}
