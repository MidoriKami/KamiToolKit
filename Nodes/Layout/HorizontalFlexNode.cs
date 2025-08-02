using System.Linq;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class HorizontalFlexNode : LayoutListNode {

    public FlexFlags AlignmentFlags { get; set; } = FlexFlags.FitContentHeight;

    public float FitPadding { get; set; } = 4.0f;

    public override float Width {
        get => base.Width;
        set {
            base.Width = value;
            RecalculateLayout();
        }
    }

    public override void RecalculateLayout() {
        var step = Width / NodeList.Count;

        if (NodeList.Count != 0 && AlignmentFlags.HasFlag(FlexFlags.FitContentHeight)) {
            Height = NodeList.Max(node => node.Height);
        }

        foreach (var index in Enumerable.Range(0, NodeList.Count)) {

            if (AlignmentFlags.HasFlag(FlexFlags.CenterHorizontally)) {
                NodeList[index].X = step * index + step / 2.0f - NodeList[index].Width / 2.0f;
            }
            else {
                NodeList[index].X = step * index;
            }

            if (AlignmentFlags.HasFlag(FlexFlags.FitHeight)) {
                NodeList[index].Height = Height;
            }

            if (AlignmentFlags.HasFlag(FlexFlags.CenterVertically)) {
                NodeList[index].Y = Height / 2 - NodeList[index].Height / 2;
            }

            if (AlignmentFlags.HasFlag(FlexFlags.FitWidth)) {
                NodeList[index].Width = step - FitPadding;
            }
        }
    }
}
