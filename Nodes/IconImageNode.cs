using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// A specialization of an ImageNode intended for displaying game icons via IconId.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public unsafe class IconImageNode : SimpleImageNode, IIconImageNode {

    /// <inheritdoc />
    public uint IconId {
        get;
        set {
            if (value != field) {
                field = value;
                PartsList[0]->LoadIcon(value);
            }
        }
    }

    /// <inheritdoc />
    public bool IsTextureReady
        => PartsList[0]->IsTextureReady;

    /// <inheritdoc />
    public uint? LoadedIconId
        => Node->IconId;

    /// <summary>
    /// Constructs a new <see cref="IconImageNode"/>
    /// </summary>
    public IconImageNode() {
        TextureSize = new Vector2(32.0f, 32.0f);
    }
}
