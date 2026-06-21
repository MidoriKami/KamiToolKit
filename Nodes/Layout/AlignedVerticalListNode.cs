using KamiToolKit.BaseTypes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="VerticalListNode"/> that ensures all nodes are lined up to horizontal X = 0.
/// </summary>
public abstract class AlignedVerticalListNode : VerticalListNode {

    /// <inheritdoc/>
    protected override void AdjustNode(NodeBase node) {
        node.X = 0.0f;
    }
}
