using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public class AlignedHorizontalListNode : HorizontalListNode {
    protected override void AdjustNode(NodeBase node) {
        node.Y = 0.0f;
    }
}
