using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Data.ListItemNodes;

/// <summary>
/// Implementation of <see cref="ListItemNode{T}"/> for displaying a loaded <see cref="AtkUnitBase"/>
/// </summary>
public class WindowListItemNode : ListItemNode<Pointer<AtkUnitBase>>, IListItemNode {

    /// <inheritdoc/>
    public static float ItemHeight => 48.0f;

    /// <summary>
    /// Gets the image node used to indicate the addon's visibility.
    /// </summary>
    protected IconImageNode VisibilityIconNode { get; }

    /// <summary>
    /// Gets the text node used for show the addons name.
    /// </summary>
    protected TextNode AddonNameTextNode { get; }

    /// <inheritdoc/>
    protected override unsafe void SetNodeData(Pointer<AtkUnitBase> itemData) {
        if (itemData.Value is null) return;

        VisibilityIconNode.MultiplyColor = itemData.Value->IsVisible ? Vector3.One : Vector3.One * 1.0f / 3.0f;
        AddonNameTextNode.String = itemData.Value->NameString;
    }

    /// <summary>
    /// Constructs a new <see cref="WindowListItemNode"/>
    /// </summary>
    public WindowListItemNode() {
        VisibilityIconNode = new IconImageNode {
            FitTexture = true,
            IconId = 240201,
        };
        VisibilityIconNode.AttachNode(this);

        AddonNameTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
        };
        AddonNameTextNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        VisibilityIconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        VisibilityIconNode.Position = new Vector2(2.0f, 2.0f);

        AddonNameTextNode.Size = new Vector2(Width - Height - 4.0f - 4.0f, Height);
        AddonNameTextNode.Position = new Vector2(VisibilityIconNode.Bounds.Right + 4.0f, 0.0f);
    }
}
