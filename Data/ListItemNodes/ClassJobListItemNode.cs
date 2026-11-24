using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UI;
using KamiToolKit.Internal;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Data.ListItemNodes;

/// <summary>
/// Implementation of <see cref="ListItemNode{T}"/> for use in <see cref="KamiToolKit.Controls.Search.ClassJobSearchAddon"/>
/// </summary>
public class ClassJobListItemNode : ListItemWithFocusNav<ClassJob>, IListItemNode {

    /// <inheritdoc/>
    public static float ItemHeight => 48.0f;

    /// <summary>
    /// Gets the icon node used to display this ClassJob's icon.
    /// </summary>
    protected IconImageNode ClassJobIconImageNode { get; }

    /// <summary>
    /// Gets the text node used to display the ClassJob's name.
    /// </summary>
    protected TextNode ClassJobNameTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the ClassJob's Abbreviation.
    /// </summary>
    protected TextNode ClassJobAbbreviationTextNode { get; }

    /// <summary>
    /// Gets the text node used to display the ClassJob's ID.
    /// </summary>
    protected TextNode ClassJobIdTextNode { get; }

    /// <inheritdoc/>
    protected override void SetNodeData(ClassJob itemData) {
        if (itemData.RowId is 0) {
            ClassJobNameTextNode.IsVisible = true;
            ClassJobNameTextNode.String = "Invalid ClassJob Id";

            ClassJobIconImageNode.IsVisible = false;
            ClassJobAbbreviationTextNode.IsVisible = false;
            ClassJobIdTextNode.IsVisible = false;
            return;
        }

        ClassJobIconImageNode.IconId = itemData.RowId + 62000;
        ClassJobIconImageNode.IsVisible = true;

        ClassJobNameTextNode.String = ISeStringEvaluator.Get().EvaluateFromAddon(698, [itemData.RowId]);
        ClassJobNameTextNode.IsVisible = true;

        ClassJobAbbreviationTextNode.String = itemData.Abbreviation;
        ClassJobAbbreviationTextNode.IsVisible = true;

        ClassJobIdTextNode.String = itemData.RowId.ToString();
        ClassJobIdTextNode.IsVisible = true;
    }

    /// <summary>
    /// Constructs a new <see cref="ClassJobListItemNode"/>
    /// </summary>
    public ClassJobListItemNode() {
        ClassJobIconImageNode = new IconImageNode {
            FitTexture = true,
        };
        ClassJobIconImageNode.AttachNode(this);

        ClassJobNameTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            AlignmentType = AlignmentType.BottomLeft,
        };
        ClassJobNameTextNode.AttachNode(this);

        ClassJobAbbreviationTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        ClassJobAbbreviationTextNode.AttachNode(this);

        ClassJobIdTextNode = new TextNode {
            TextFlags = TextFlags.Emboss,
            FontSize = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor = ColorHelper.GetColor(3),
        };
        ClassJobIdTextNode.AttachNode(this);

        KamiToolKitLibrary.PluginInterface.Inject(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ClassJobIconImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        ClassJobIconImageNode.Position = new Vector2(2.0f, 2.0f);

        ClassJobIdTextNode.Size = new Vector2(48.0f, Height / 2.0f);
        ClassJobIdTextNode.Position = new Vector2(Width - ClassJobIdTextNode.Width, 0.0f);

        ClassJobNameTextNode.Size = new Vector2(Width - ClassJobIconImageNode.Width - ClassJobIdTextNode.Width - 8.0f, Height / 2.0f);
        ClassJobNameTextNode.Position = new Vector2(ClassJobIconImageNode.Bounds.Right + 4.0f, 0.0f);

        ClassJobAbbreviationTextNode.Size = ClassJobNameTextNode.Size;
        ClassJobAbbreviationTextNode.Position = ClassJobNameTextNode.Position with { Y = Height / 2.0f };
    }
}
