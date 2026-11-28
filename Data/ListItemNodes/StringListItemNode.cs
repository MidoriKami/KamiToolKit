using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Data.ListItemNodes;

/// <summary>
/// Implementation of a <see cref="ListItemNode{T}"/> that represents a string.
/// </summary>
public class StringListItemNode : ListItemNode<string>, IListItemNode {

    /// <inheritdoc/>
    public static float ItemHeight => 24.0f;

    /// <summary>
    /// The text node used to display this string.
    /// </summary>
    protected TextNode LabelTextNode { get; }

    /// <inheritdoc/>
    protected override void SetNodeData(string itemData)
        => LabelTextNode.String = itemData;

    /// <summary>
    /// Constructs a new <see cref="StringListItemNode"/>
    /// </summary>
    public StringListItemNode() {
        LabelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
        };
        LabelTextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelTextNode.Size = new Vector2(Width - 8.0f, Height);
        LabelTextNode.Position = new Vector2(8.0f, 0.0f);
    }
}
