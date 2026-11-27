using System.Linq;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Interfaces.Nodes;

namespace KamiToolKit.UI.Nodes.Layout;

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
            UpdateLayout();
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateSizes() {
        if (AlignmentFlags.HasFlag(FlexFlags.FitContentHeight)) {
            Height = NodeList.Max(node => node.Height);
        }

        var step = Width / NodeList.Count;
        foreach (var index in Enumerable.Range(0, NodeList.Count)) {
            if (AlignmentFlags.HasFlag(FlexFlags.FitHeight)) {
                NodeList[index].Height = Height;
            }

            if (AlignmentFlags.HasFlag(FlexFlags.FitWidth)) {
                NodeList[index].Width = step - ItemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        var step = Width / NodeList.Count;
        foreach (var index in Enumerable.Range(0, NodeList.Count)) {
            if (AlignmentFlags.HasFlag(FlexFlags.CenterHorizontally)) {
                NodeList[index].X = step * index + step / 2.0f - NodeList[index].Width / 2.0f;
            }
            else {
                NodeList[index].X = step * index;
            }

            if (AlignmentFlags.HasFlag(FlexFlags.CenterVertically)) {
                NodeList[index].Y = Height / 2 - NodeList[index].Height / 2;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
        var navigationGroups = GetNavigationChildGroups();

        ConnectHorizontalNavigationGroups(navigationGroups);

        var boundary = CreateNavigationBoundary(navigationGroups);
        SetNavigationBoundary(boundary);
        LinkNavigationBoundaryToIndex(boundary.Left, ControllerNavigationDirection.Left, NavLeft);
        LinkNavigationBoundaryToIndex(boundary.Right, ControllerNavigationDirection.Right, NavRight);

        foreach (var group in navigationGroups) {
            LinkNavigationBoundaryToIndex(group.Boundary.Top, ControllerNavigationDirection.Up, NavUp);
            LinkNavigationBoundaryToIndex(group.Boundary.Bottom, ControllerNavigationDirection.Down, NavDown);
        }
    }
}
