using System;
using System.Linq;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

public class OrderedVerticalListNode<T, TU> : VerticalListNode where T : NodeBase {

    public Func<T, TU>? OrderSelector { get; set; }

    protected override void OnRecalculateLayout() {
        var typedList = NodeList.OfType<T>();

        if (OrderSelector is null) {
            RecalculateLayout();
            return;
        }

        var orderedList = typedList.OrderBy(OrderSelector).ToList();

        var startY = Anchor switch {
            VerticalListAnchor.Top => 0.0f + FirstItemSpacing,
            VerticalListAnchor.Bottom => Height,
            _ => 0.0f,
        };

        foreach (var node in orderedList) {
            if (!node.IsVisible) continue;

            if (Anchor is VerticalListAnchor.Bottom) {
                startY -= node.Height + ItemSpacing;
            }

            node.Y = startY;
            AdjustNode(node);

            if (Anchor is VerticalListAnchor.Top) {
                startY += node.Height + ItemSpacing;
            }
        }

        if (FitContents) {
            Height = orderedList.Sum(node => node.IsVisible ? node.Height + ItemSpacing : 0.0f) + FirstItemSpacing;
        }
    }
}
