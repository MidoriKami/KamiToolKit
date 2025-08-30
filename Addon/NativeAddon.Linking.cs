using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Addon;

public abstract partial class NativeAddon {
    protected void AttachNode(NodeBase node, NodePosition? position = null)
        => NativeController.AttachNode(node, this, position);

    protected void AttachNode(NodeBase node, NodeBase target, NodePosition? position = null)
        => NativeController.AttachNode(node, target, position);
}
