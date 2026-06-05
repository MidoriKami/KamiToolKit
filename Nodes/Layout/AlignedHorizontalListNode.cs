using KamiToolKit.BaseTypes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="HorizontalListNode"/> that ensures everything is lined up to vertical Y = 0.
/// </summary>
public class AlignedHorizontalListNode : HorizontalListNode {
    protected override void AdjustNode(NodeBase node) {
        node.Y = 0.0f;
    }
}
