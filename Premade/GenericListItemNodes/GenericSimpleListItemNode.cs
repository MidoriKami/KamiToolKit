using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.GenericListItemNodes;

public abstract class GenericSimpleListItemNode<T> : ListItemNode<T> {
    public override float ItemHeight => 48.0f;
    
    protected readonly IconImageNode IconNode;
    protected readonly TextNode LabelTextNode;

    protected GenericSimpleListItemNode() {
        IconNode = new IconImageNode {
            FitTexture = true,
            IconId = 60072,
        };
        IconNode.AttachNode(this);

        LabelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            LineSpacing = 14,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        LabelTextNode.AttachNode(this);

        CollisionNode.ShowClickableCursor = true;
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        IconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        IconNode.Position = new Vector2(2.0f, 2.0f);

        LabelTextNode.Size = new Vector2(Width - IconNode.Width - 6.0f, Height);
        LabelTextNode.Position = new Vector2(IconNode.Bounds.Right + 6.0f, 0.0f);
    }
}
