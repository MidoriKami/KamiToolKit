using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.GenericListItemNodes;

public abstract class GenericStringListItemNode<T> : ListItemNode<T> {
    public override float ItemHeight => 24.0f;
    
    protected readonly TextNode StringNode;

    protected GenericStringListItemNode() {
        StringNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            LineSpacing = 14,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        StringNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        StringNode.Size = new Vector2(Width - 20.0f, Height);
        StringNode.Position = new Vector2(10.0f, 0.0f);
    }
}
