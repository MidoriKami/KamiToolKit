using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

public static unsafe class AtkImageNodeExtensions {
    public static uint GetIconId(this ref AtkImageNode node) {
        if (node.PartsList is null) return 0;
        if (node.PartsList->Parts is null) return 0;
        if (node.PartsList->Parts->UldAsset is null) return 0;
        if (node.PartsList->Parts->UldAsset->AtkTexture.TextureType is not TextureType.Resource) return 0;
        if (node.PartsList->Parts->UldAsset->AtkTexture.Resource is null) return 0;

        return node.PartsList->Parts->UldAsset->AtkTexture.Resource->IconId;
    }
}
