using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public class SimpleComponentNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase> {
    public override SeString? Tooltip {
        get => CollisionNode.Tooltip;
        set => CollisionNode.Tooltip = value;
    }
}
