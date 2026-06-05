using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

/// <summary>
/// A specialization of an ImageNode intended for displaying game icons via IconId.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public unsafe class IconImageNode : SimpleImageNode {

    /// <summary>
    /// Gets or sets the displayed Icon.
    /// </summary>
    public uint IconId {
        get;
        set {
            if (value != field) {
                field = value;
                PartsList[0]->LoadIcon(value);
            }
        }
    }

    /// <summary>
    /// Gets if the texture is loaded.
    /// </summary>
    public bool IsTextureReady
        => PartsList[0]->IsTextureReady;

    /// <summary>
    /// Gets the currently loaded iconId
    /// </summary>
    public uint? LoadedIconId
        => Node->IconId;

    public IconImageNode() {
        TextureSize = new Vector2(32.0f, 32.0f);
    }
}
