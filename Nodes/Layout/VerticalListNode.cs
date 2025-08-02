using System.Linq;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public class VerticalListNode : LayoutListNode {

    [JsonProperty] public VerticalListAnchor Alignment {
        get;
        set {
            field = value;
            RecalculateLayout();
        }
    }

    // Resizes this node to fit all elements
    public bool FitContents { get; set; }

    public override void RecalculateLayout() {
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
