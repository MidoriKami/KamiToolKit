using System;
using System.Linq;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class OrderedVerticalListNode<T, TU> : VerticalListNode where T : NodeBase {

    public Func<T, TU>? OrderSelector { get; set; }

    public override void RecalculateLayout() {
        var typedList = NodeList.OfType<T>();

        if (OrderSelector is null) {
            base.RecalculateLayout();
            return;
        }

        var orderedList = typedList.OrderBy(OrderSelector).ToList();

        var startY = Alignment switch {
            VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
            VerticalListAnchor.Bottom => Height,
            _ => 0.0f,
        };

        foreach (var node in orderedList) {
            if (!node.IsVisible) continue;

            if (Alignment is VerticalListAnchor.Bottom) {
                startY -= node.Height + ItemSpacing;
            }

            node.Y = startY;
            AdjustNode(node);

            if (Alignment is VerticalListAnchor.Top) {
                startY += node.Height + ItemSpacing;
            }
        }

        if (FitContents) {
            Height = orderedList.Sum(node => node.IsVisible ? node.Height + ItemSpacing : 0.0f) + FirstItemSpacing;
        }
    }
}
