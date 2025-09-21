using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUldPartExtensions {
    public static void LoadTexture(ref this AtkUldPart part, string path, bool resolveTheme = true) {
        try {
            if (part.UldAsset is null) return;

            part.UldAsset->AtkTexture.ReleaseTexture();
            part.UldAsset->AtkTexture.TextureType = 0;

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

    public static void LoadIcon(ref this AtkUldPart part, uint iconId, IconSubFolder? alternateFolder = null) {
        var iconPath = GetPathForIcon(iconId, alternateFolder);
        if (iconPath != string.Empty) {
            part.LoadTexture(GetPathForIcon(iconId, alternateFolder));
        }
        else {
            Log.Warning($"Unable to get texture path for icon: {iconId}");
        }
    }

    public static void LoadTexture(ref this AtkUldPart part, Texture* texture) {
        if (part.UldAsset is null) return;
        
        part.UldAsset->AtkTexture.ReleaseTexture();
        part.UldAsset->AtkTexture.TextureType = 0;

        part.UldAsset->AtkTexture.KernelTexture = texture;
        part.UldAsset->AtkTexture.TextureType = TextureType.KernelTexture;
    }

    public static void LoadTexture(ref this AtkUldPart part, IDalamudTextureWrap textureWrap) {
        var texturePointer = (Texture*)DalamudInterface.Instance.TextureProvider.ConvertToKernelTexture(textureWrap, true);
        part.LoadTexture(texturePointer);
    }

    private static string GetThemePathModifier() => AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType switch {
        not 0 => $"uld/img{AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType:00}",
        _ => "uld",
    };

    public static string GetLoadedPath(ref this AtkUldPart part) {
        if (part.UldAsset is null) return string.Empty;
        if (part.UldAsset->AtkTexture.Resource is null) return string.Empty;
        if (part.UldAsset->AtkTexture.Resource->TexFileResourceHandle is null) return string.Empty;

        return part.UldAsset->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();
    }
    
    public static string GetPathForIcon(uint iconId, IconSubFolder? alternateFolder = null) {
        var textureManager = AtkStage.Instance()->AtkTextureResourceManager;
        Span<byte> buffer = stackalloc byte[0x100];
        buffer.Clear();
        var bytePointer = (byte*) Unsafe.AsPointer(ref buffer[0]);

        var textureScale = textureManager->DefaultTextureScale;
        alternateFolder ??= (IconSubFolder)textureManager->IconLanguage;
        
        // Try to resolve the path using the current language
        AtkTexture.GetIconPath(bytePointer, iconId, textureScale, alternateFolder.Value);
        var pathResult = GetString(bytePointer);

        // If the resolved path doesn't exist, re-process with default folder
        if (!DalamudInterface.Instance.DataManager.FileExists(pathResult)) {
            AtkTexture.GetIconPath(bytePointer, iconId, textureScale, 0);
            pathResult = GetString(bytePointer);
        }

        return DalamudInterface.Instance.DataManager.FileExists(pathResult) ? pathResult : string.Empty;
    }
    
    private static string GetString(byte* buffer)
        => MemoryMarshal.CreateReadOnlySpanFromNullTerminated(buffer).GetString();
}
