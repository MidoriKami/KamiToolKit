using System.Linq;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A horizontal <see cref="LayoutListNode"/> that will evenly divide its area among its children nodes.
/// </summary>
public class HorizontalFlexNode : LayoutListNode, IHorizontalFlexNode {

    /// <inheritdoc />
    public FlexFlags AlignmentFlags { get; set; } = FlexFlags.FitContentHeight;

    /// <inheritdoc />
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Horizontal;

    /// <inheritdoc/>
    public override float Width {
        get => base.Width;
        set {
            base.Width = value;
            RecalculateLayout();
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
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
                NodeList[index].Width = step - ItemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        var componentNodes = NodeList.OfType<ComponentNode>().ToList();
        if (componentNodes.Count is 0) return;

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
