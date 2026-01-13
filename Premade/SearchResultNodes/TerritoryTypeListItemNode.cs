using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.SearchResultNodes;

public class TerritoryTypeListItemNode : ListItemNode<TerritoryType> {
    public override float ItemHeight => 64.0f;

    private readonly SimpleImageNode territoryImageNode;
    private readonly TextNode territoryTitleNode;
    private readonly TextNode territoryDescriptionNode;

    public TerritoryTypeListItemNode() {
        territoryImageNode = new SimpleImageNode {
            FitTexture = true,
        };
        territoryImageNode.AttachNode(this);

        territoryTitleNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.BottomLeft,
        };
        territoryTitleNode.AttachNode(this);

        territoryDescriptionNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(2),
        };
        territoryDescriptionNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        territoryImageNode.Size = new Vector2((Height - 4.0f) * 1.777f, Height - 4.0f);
        territoryImageNode.Position = new Vector2(2.0f, 2.0f);

        territoryTitleNode.Size = new Vector2(Width - territoryImageNode.Width - 10.0f, Height / 2.0f);
        territoryTitleNode.Position = new Vector2(territoryImageNode.Bounds.Right + 8.0f, 0.0f);

        territoryDescriptionNode.Size = territoryTitleNode.Size;
        territoryDescriptionNode.Position = new Vector2(territoryTitleNode.Bounds.Left, Height / 2.0f);
    }

    protected override void SetNodeData(TerritoryType territory) {
        if (territory.RowId is 0) return;
        
        if (territory.LoadingImage.ValueNullable?.FileName is { IsEmpty: false } filePath) {
            territoryImageNode.LoadTexture($"ui/loadingimage/{filePath}_hr1.tex");
        }

        territoryTitleNode.String = territory.PlaceName.ValueNullable?.Name.ToString() ?? string.Empty;
        territoryDescriptionNode.String = territory.ContentFinderCondition.RowId is 0 ? string.Empty : territory.ContentFinderCondition.Value.Name.ToString();
    }
}
