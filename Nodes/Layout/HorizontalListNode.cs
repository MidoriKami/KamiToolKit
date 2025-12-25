using System.Linq;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class HorizontalListNode : LayoutListNode {

    public HorizontalListAnchor Alignment {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    public override float Width {
        get => base.Width;
        set {
            base.Width = value;
            RecalculateLayout();
        }
    }
    
    public bool FitToContentHeight { get; set; }

    protected override void InternalRecalculateLayout() {
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

        if (FitToContentHeight) {
            Height = NodeList.Max(node => node.Height);
        }
    }
    
    public float AreaRemaining => Width - NodeList.Sum(node => node.Width + ItemSpacing) - ItemSpacing;
}
