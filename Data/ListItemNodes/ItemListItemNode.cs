using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Classes;
using KamiToolKit.Controls.Search;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Data.ListItemNodes;

/// <summary>
/// Specialization of <see cref="ListItemNode{T}"/> for use with <see cref="ListNode{T,TU}"/> or <see cref="ItemSearchAddon"/>
/// to represent <see cref="Item"/>'s.
/// </summary>
public class ItemListItemNode : ListItemWithFocusNav<Item>, IListItemNode {

    /// <inheritdoc/>
    public static float ItemHeight => 48.0f;

    /// <summary>
    /// Gets or sets whether mousing over the icon will show the item's tooltip.
    /// </summary>
    public bool EnableItemTooltip { get; set; } = true;

    /// <summary>
    /// Gets the icon node used to display this item.
    /// </summary>
    protected IconImageNode ItemIconImageNode { get; }

    /// <summary>
    /// Gets the text node used to display the items name.
    /// </summary>
    protected TextNode ItemNameTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the items ItemUiCategory.
    /// </summary>
    protected TextNode ItemCategoryTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the items ID.
    /// </summary>
    protected TextNode ItemIdTextNode { get; }

    /// <inheritdoc/>
    protected override void SetNodeData(Item itemData) {
        if (itemData.RowId is 0) {
            ItemNameTextNode.IsVisible = true;
            ItemNameTextNode.String = "Invalid Item Id";

            ItemIconImageNode.IsVisible = false;
            ItemCategoryTextNode.IsVisible = false;
            ItemIdTextNode.IsVisible = false;
            return;
        }

        ItemIconImageNode.IconId = itemData.Icon;
        ItemIconImageNode.ItemTooltip = EnableItemTooltip ? itemData.RowId : 0;
        ItemIconImageNode.ShowClickableCursor = EnableItemTooltip;
        ItemIconImageNode.IsVisible = true;

        ItemNameTextNode.String = itemData.Name;
        ItemNameTextNode.IsVisible = true;

        ItemCategoryTextNode.String = itemData.ItemSearchCategory.ValueNullable?.Name ?? new ReadOnlySeString();
        ItemCategoryTextNode.IsVisible = true;

        ItemIdTextNode.String = itemData.RowId.ToString();
        ItemIdTextNode.IsVisible = true;
    }

    /// <summary>
    /// Constructs a new <see cref="ItemListItemNode"/>
    /// </summary>
    public ItemListItemNode() {
        ItemIconImageNode = new IconImageNode {
            FitTexture = true,
            ItemTooltip = 1,
        };
        ItemIconImageNode.AttachNode(this);

        ItemNameTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            AlignmentType = AlignmentType.BottomLeft,
        };
        ItemNameTextNode.AttachNode(this);

        ItemCategoryTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        ItemCategoryTextNode.AttachNode(this);

        ItemIdTextNode = new TextNode {
            TextFlags = TextFlags.Emboss,
            FontSize = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor = ColorHelper.GetColor(3),
        };
        ItemIdTextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ItemIconImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        ItemIconImageNode.Position = new Vector2(2.0f, 2.0f);

        ItemIdTextNode.Size = new Vector2(48.0f, Height / 2.0f);
        ItemIdTextNode.Position = new Vector2(Width - ItemIdTextNode.Width, 0.0f);

        ItemNameTextNode.Size = new Vector2(Width - ItemIconImageNode.Width - ItemIdTextNode.Width - 8.0f, Height / 2.0f);
        ItemNameTextNode.Position = new Vector2(ItemIconImageNode.Bounds.Right + 4.0f, 0.0f);

        ItemCategoryTextNode.Size = ItemNameTextNode.Size;
        ItemCategoryTextNode.Position = ItemNameTextNode.Position with { Y = Height / 2.0f };
    }

    /// <inheritdoc />
    protected override void OnNavHoverStart() {
        base.OnNavHoverStart();

        if (EnableItemTooltip) {
            ItemIconImageNode.ShowTooltip();
        }
    }

    /// <inheritdoc />
    protected override void OnNavHoverEnd() {
        base.OnNavHoverEnd();

        ItemIconImageNode.HideTooltip();
    }
}
