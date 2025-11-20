using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

/// <summary>
/// WARNING: This is a non-owning texture image node.
/// This node is meant to reference a texture that is owned elsewhere.
/// </summary>
public unsafe class TextureImageNode : SimpleImageNode {
    public void SetTexture(Texture* texture) {
        var asset = PartsList[0]->UldAsset;
        asset->AtkTexture.KernelTexture = texture;
        asset->AtkTexture.TextureType = TextureType.KernelTexture;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            var asset = PartsList[0]->UldAsset;
            asset->AtkTexture.KernelTexture = null;
            asset->AtkTexture.TextureType = 0;
            
            base.Dispose(disposing, isNativeDestructor);
        }
    }
}
