using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.ListItemNodes;

public class StatusListItemNode : ListItemNode<Status> {
    public override float ItemHeight => 48.0f;

    protected readonly IconImageNode IconImageNode;
    protected readonly TextNode StatusLabelNode;
    
    public StatusListItemNode() {
        IconImageNode = new IconImageNode {
            FitTexture = true,
            IconId = 60072,
        };
        IconImageNode.AttachNode(this);

        StatusLabelNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            LineSpacing = 14,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        StatusLabelNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        IconImageNode.Size = new Vector2((Height - 4.0f) * 0.75f , Height - 4.0f);
        IconImageNode.Position = new Vector2(2.0f, 2.0f);

        StatusLabelNode.Size = new Vector2(Width - IconImageNode.Width - 6.0f, Height);
        StatusLabelNode.Position = new Vector2(IconImageNode.Bounds.Right + 6.0f, 0.0f);
    }

    protected override void SetNodeData(Status itemData) {
        IconImageNode.IconId = itemData.Icon;
        StatusLabelNode.String = itemData.Name;
    }
}
