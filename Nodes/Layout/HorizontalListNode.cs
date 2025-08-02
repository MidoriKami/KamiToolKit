using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public class HorizontalListNode : LayoutListNode {

    [JsonProperty] public HorizontalListAnchor Alignment {
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

    public override void RecalculateLayout() {
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
}
