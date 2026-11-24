using System.Numerics;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Classes;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Data.ListItemNodes;

/// <summary>
/// Implementation of <see cref="ListItemNode{T}"/> for use with <see cref="KamiToolKit.Controls.Search.GearsetSearchAddon"/>
/// </summary>
public class GearsetListItemNode : ListItemWithFocusNav<RaptureGearsetModule.GearsetEntry>, IListItemNode {

    /// <inheritdoc/>
    public static float ItemHeight => 48.0f;

    /// <summary>
    /// Gets the icon node used to display this item.
    /// </summary>
    protected IconImageNode ItemIconImageNode { get; }

    /// <summary>
    /// Gets the text node used to display the items name.
    /// </summary>
    protected TextNode GearsetNameTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the items ItemUiCategory.
    /// </summary>
    protected TextNode GearsetItemLevelTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the items ID.
    /// </summary>
    protected TextNode GearsetIndexTextNode { get; }

    /// <inheritdoc/>
    protected override void SetNodeData(RaptureGearsetModule.GearsetEntry itemData) {
        if (!itemData.Flags.HasFlag(RaptureGearsetModule.GearsetFlag.Exists)) {
            GearsetNameTextNode.IsVisible = true;
            GearsetNameTextNode.String = "Invalid Gearset Id";

            ItemIconImageNode.IsVisible = false;
            GearsetItemLevelTextNode.IsVisible = false;
            GearsetIndexTextNode.IsVisible = false;
            return;
        }

        ItemIconImageNode.IconId = (uint) (itemData.ClassJob + 62000);
        ItemIconImageNode.IsVisible = true;

        GearsetNameTextNode.String = itemData.NameString;
        GearsetNameTextNode.IsVisible = true;

        GearsetItemLevelTextNode.String = $"{SeIconChar.ItemLevel.ToIconString()}{itemData.ItemLevel}";
        GearsetItemLevelTextNode.IsVisible = true;

        GearsetIndexTextNode.String = $"{itemData.Id + 1}";
        GearsetIndexTextNode.IsVisible = true;
    }

    /// <summary>
    /// Constructs a new <see cref="GearsetListItemNode"/>
    /// </summary>
    public GearsetListItemNode() {
        ItemIconImageNode = new IconImageNode {
            FitTexture = true,
        };
        ItemIconImageNode.AttachNode(this);

        GearsetNameTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            AlignmentType = AlignmentType.BottomLeft,
        };
        GearsetNameTextNode.AttachNode(this);

        GearsetItemLevelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        GearsetItemLevelTextNode.AttachNode(this);

        GearsetIndexTextNode = new TextNode {
            TextFlags = TextFlags.Emboss,
            FontSize = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor = ColorHelper.GetColor(3),
        };
        GearsetIndexTextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ItemIconImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        ItemIconImageNode.Position = new Vector2(2.0f, 2.0f);

        GearsetIndexTextNode.Size = new Vector2(48.0f, Height / 2.0f);
        GearsetIndexTextNode.Position = new Vector2(Width - GearsetIndexTextNode.Width, 0.0f);

        GearsetNameTextNode.Size = new Vector2(Width - ItemIconImageNode.Width - GearsetIndexTextNode.Width - 8.0f, Height / 2.0f);
        GearsetNameTextNode.Position = new Vector2(ItemIconImageNode.Bounds.Right + 4.0f, 0.0f);

        GearsetItemLevelTextNode.Size = GearsetNameTextNode.Size;
        GearsetItemLevelTextNode.Position = GearsetNameTextNode.Position with { Y = Height / 2.0f };
    }
}
