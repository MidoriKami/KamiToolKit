using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// WARNING: This is a non-owning texture image node.
/// This node is meant to reference a texture that is owned elsewhere.
/// </summary>
/// <remarks>
/// This node is experimental, use at your own risk.
/// </remarks>
[Experimental("KTKExperimental_TextureImageNode")]
public unsafe class TextureImageNode : SimpleImageNode {

    /// <summary>
    /// Sets the used texture of this node to the set texture.
    /// </summary>
    /// <remarks>
    /// If this texture somehow gets disposed by this node, memory leaks, and other undefined behavior may occur.
    /// </remarks>
    public void SetTexture(Texture* texture) {
        var asset = PartsList[0]->UldAsset;
        asset->AtkTexture.KernelTexture = texture;
        asset->AtkTexture.TextureType = TextureType.KernelTexture;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing && !IsDisposed) {
            if (!isNativeDestructor) {
                var asset = PartsList[0]->UldAsset;
                asset->AtkTexture.KernelTexture = null;
                asset->AtkTexture.TextureType = 0;
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }
}
