using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUldPartExtensions {
    extension(ref AtkUldPart part) {
        public bool IsTextureReady => part.UldAsset is not null && part.UldAsset->AtkTexture.IsTextureReady();
        public Vector2 LoadedTextureSize => part.GetActualTextureSize();
        public string LoadedPath => part.GetLoadedPath();
        
        public void LoadTexture(string path, bool resolveTheme = true) {
            try {
                if (part.UldAsset is null) return;

                part.TryUnloadTexture();

                var texturePath = path.Replace("_hr1", string.Empty);

                var themedPath = texturePath.Replace("uld", GetThemePathModifier());
                if (DalamudInterface.Instance.DataManager.FileExists(themedPath) && resolveTheme) {
                    texturePath = themedPath;
                }

                if (DalamudInterface.Instance.DataManager.FileExists(texturePath)) {
                    part.UldAsset->AtkTexture.LoadTextureWithDefaultVersion(texturePath);
                }
            }
            catch (Exception e) {
                Log.Exception(e);
            }
        }

        public void LoadIcon(uint iconId)
            => part.UldAsset->AtkTexture.LoadIconTexture(iconId, GetIconSubFolder(iconId));
        
        private Vector2 GetActualTextureSize() {
            if (part.UldAsset is null) return Vector2.Zero;
            if (!part.UldAsset->AtkTexture.IsTextureReady()) return Vector2.Zero;
            if (part.UldAsset->AtkTexture.TextureType is 0) return Vector2.Zero;
            if (part.UldAsset->AtkTexture.KernelTexture is null) return Vector2.Zero;

            var width = part.UldAsset->AtkTexture.GetTextureWidth();
            var height = part.UldAsset->AtkTexture.GetTextureHeight();
            return new Vector2(width, height);
        }

        public void LoadTexture(Texture* texture) {
            if (part.UldAsset is null) return;
        
            part.TryUnloadTexture();
            part.UldAsset->AtkTexture.KernelTexture = texture;
            part.UldAsset->AtkTexture.TextureType = TextureType.KernelTexture;
        }

        public void LoadTexture(IDalamudTextureWrap textureWrap) {
            var texturePointer = (Texture*)DalamudInterface.Instance.TextureProvider.ConvertToKernelTexture(textureWrap, true);
            if (texturePointer is null) return;
            
            part.LoadTexture(texturePointer);
        }
        
        private string GetLoadedPath() {
            if (part.UldAsset is null) return string.Empty;
            if (part.UldAsset->AtkTexture.Resource is null) return string.Empty;
            if (part.UldAsset->AtkTexture.Resource->TexFileResourceHandle is null) return string.Empty;

            return part.UldAsset->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();
        }

        private void TryUnloadTexture() {
            if (part.UldAsset is null) return;
            if (!part.UldAsset->AtkTexture.IsTextureReady()) return;
            if (part.UldAsset->AtkTexture.TextureType is 0) return;
            if (part.UldAsset->AtkTexture.KernelTexture is null) return;

            part.UldAsset->AtkTexture.ReleaseTexture();
            part.UldAsset->AtkTexture.KernelTexture = null;
            part.UldAsset->AtkTexture.TextureType = 0;
        }
    }

    private static string GetThemePathModifier() => AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType switch {
        not 0 => $"uld/img{AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType:00}",
        _ => "uld",
    };

    public static IconSubFolder GetIconSubFolder(uint iconId) {
        var textureManager = AtkStage.Instance()->AtkTextureResourceManager;
        Span<byte> buffer = stackalloc byte[0x100];
        buffer.Clear();
        var bytePointer = (byte*) Unsafe.AsPointer(ref buffer[0]);

        var textureScale = textureManager->DefaultTextureScale;
        var targetFolder = (IconSubFolder)textureManager->IconLanguage;
        
        // Try to resolve the path using the current language
        AtkTexture.GetIconPath(bytePointer, iconId, textureScale, targetFolder);
        var pathResult = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(bytePointer).String;

        // If the resolved path doesn't exist, re-process with default folder
        return DalamudInterface.Instance.DataManager.FileExists(pathResult) ? targetFolder : IconSubFolder.None;
    }
}
