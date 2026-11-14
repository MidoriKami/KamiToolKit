using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// A simplified implementation of a <see cref="ComponentNode"/>
/// </summary>
/// <remarks>
/// Warning, do not use this node to represent a layouting structural node. Use ResNode instead.
/// </remarks>
public class SimpleComponentNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase> {

    /// <summary>
    /// Initializes a new instance of <see cref="SimpleComponentNode"/>
    /// </summary>
    public SimpleComponentNode()
        => InitializeComponentEvents();

    /// <inheritdoc/>
    public override ReadOnlySeString TextTooltip {
        get => CollisionNode.TextTooltip;
        set => CollisionNode.TextTooltip = value;
    }
}
