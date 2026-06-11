using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for AtkImageNode.
/// </summary>
public static unsafe class AtkImageNodeExtensions {
    extension(ref AtkImageNode node) {

        /// <summary>
        /// Gets the currently used IconId for this node.
        /// </summary>
        /// <remarks>
        /// Zero if unset or invalid.
        /// </remarks>
        public uint IconId => node.GetIconId();

        private uint GetIconId() {
            if (node.PartsList is null) return 0;
            if (node.PartsList->Parts is null) return 0;
            if (node.PartsList->Parts->UldAsset is null) return 0;
            if (node.PartsList->Parts->UldAsset->AtkTexture.TextureType is not TextureType.Resource) return 0;
            if (node.PartsList->Parts->UldAsset->AtkTexture.Resource is null) return 0;

            return node.PartsList->Parts->UldAsset->AtkTexture.Resource->IconId;
        }
    }
}
