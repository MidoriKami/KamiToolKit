using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Premade.Node.Simple;

public class SimpleComponentNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase> {
    public override ReadOnlySeString TextTooltip {
        get => CollisionNode.TextTooltip;
        set => CollisionNode.TextTooltip = value;
    }

    public bool DisableCollisionNode {
        set {
            if (!value) {
                throw new Exception("Clearing DisableCollisionNode is not supported.");
            }

            CollisionNode.NodeFlags = 0;
        }
    }
}
