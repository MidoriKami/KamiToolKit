using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;

namespace KamiToolKit.NodeParts;

/// <summary>
///     Wrapper around a AtkUldPart and AtkUldAsset, loads and holds graphics textures for display in image, and image-like
///     nodes.
/// </summary>
public unsafe class Part : IDisposable {

    private AtkUldAsset* internalAsset;

    internal AtkUldPart* InternalPart;

    private bool isDisposed;

    public Part() {
        InternalPart = NativeMemoryHelper.UiAlloc<AtkUldPart>();

        InternalPart->Width = 0;
        InternalPart->Height = 0;
        InternalPart->U = 0;
        InternalPart->V = 0;

        internalAsset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();

        internalAsset->Id = 0;
        internalAsset->AtkTexture.Ctor();

        InternalPart->UldAsset = internalAsset;
    }

    public float Width {
        get => InternalPart->Width;
        set => InternalPart->Width = (ushort)value;
    }

    public float Height {
        get => InternalPart->Height;
        set => InternalPart->Height = (ushort)value;
    }

    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public float U {
        get => InternalPart->U;
        set => InternalPart->U = (ushort)value;
    }

    public float V {
        get => InternalPart->V;
        set => InternalPart->V = (ushort)value;
    }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public uint Id {
        get => internalAsset->Id;
        set => internalAsset->Id = value;
    }

    public string TexturePath {
        get => GetLoadedPath();
        set => LoadTexture(value);
    }

    public void Dispose() {
        if (!isDisposed) {
            internalAsset->AtkTexture.ReleaseTexture();
            internalAsset->AtkTexture.Destroy(true);

            NativeMemoryHelper.UiFree(internalAsset);
            internalAsset = null;

            NativeMemoryHelper.UiFree(InternalPart);
            InternalPart = null;
        }

        isDisposed = true;
    }

    /// <summary>
    ///     Gets the loaded tex file resource handle texture, string.Empty if null
    /// </summary>
    /// <returns></returns>
    public string GetLoadedPath() {
        if (internalAsset is null) return string.Empty;
        if (internalAsset->AtkTexture.Resource is null) return string.Empty;
        if (internalAsset->AtkTexture.Resource->TexFileResourceHandle is null) return string.Empty;

        return internalAsset->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();
    }

    /// <summary>
    ///     Load the specified path, this will try to select the correct path based on theme
    ///     and resolve through any texture substitutions defined by other plugins.
    /// </summary>
    /// <param name="path">Path to load</param>
    /// <param name="resolveTheme">If set to false, will not load a themed version of the texture</param>
    public void LoadTexture(string path, bool resolveTheme = true) {
        try {
            internalAsset->AtkTexture.ReleaseTexture();

            var texturePath = path.Replace("_hr1", string.Empty);

            var themedPath = texturePath.Replace("uld", GetThemePathModifier());
            if (DalamudInterface.Instance.DataManager.FileExists(themedPath) && resolveTheme) {
                texturePath = themedPath;
            }

            internalAsset->AtkTexture.LoadTextureWithDefaultVersion(texturePath);
        }
        catch (Exception e) {
            Log.Exception(e);
        }
    }

    private string GetThemePathModifier() => AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType switch {
        not 0 => $"uld/img{AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType:00}",
        _ => "uld",
    };

    /// <summary>
    ///     Loads a game icon via id
    /// </summary>
    /// <param name="iconId">Icon id to load</param>
    /// <param name="alternateFolder">Language folder to try and load icon from</param>
    public void LoadIcon(uint iconId, IconSubFolder? alternateFolder = null) 
        => LoadTexture(GetPathForIcon(iconId, alternateFolder));

    /// <summary>
    ///     Loads texture via an already constructed Texture*
    /// </summary>
    /// <remarks>This does not preserve any existing texture.</remarks>
    /// <param name="texture">Texture to assign to this image node.</param>
    public void LoadTexture(Texture* texture) {
        internalAsset->AtkTexture.ReleaseTexture();

        internalAsset->AtkTexture.KernelTexture = texture;
        internalAsset->AtkTexture.TextureType = TextureType.KernelTexture;
    }

    /// <summary>
    ///     Loads a texture via dalamud texture wrap.
    /// </summary>
    /// <remarks>WIP</remarks>
    /// <param name="texture">Texture Wrap to convert</param>
    public void LoadTexture(IDalamudTextureWrap texture) {
        var texturePointer = (Texture*)DalamudInterface.Instance.TextureProvider.ConvertToKernelTexture(texture, true);
        LoadTexture(texturePointer);
    }

    public static string GetPathForIcon(uint iconId, IconSubFolder? alternateFolder = null) {
        var textureManager = AtkStage.Instance()->AtkTextureResourceManager;
        Span<byte> buffer = stackalloc byte[0x100];
        buffer.Clear();
        var bytePointer = (byte*) Unsafe.AsPointer(ref buffer[0]);

        var textureScale = textureManager->DefaultTextureScale;
        alternateFolder ??= (IconSubFolder)textureManager->IconLanguage;
        
        // Try to resolve the path using the current language
        Experimental.Instance.GetIconPath?.Invoke(bytePointer, iconId, textureScale, alternateFolder.Value);
        var pathResult = GetString(bytePointer);

        // If the resolved path doesn't exist, re-process with default folder
        if (!DalamudInterface.Instance.DataManager.FileExists(pathResult)) {
            Experimental.Instance.GetIconPath?.Invoke(bytePointer, iconId, textureScale, 0);
            pathResult = GetString(bytePointer);
        }
        
        return pathResult;
    }

    private static string GetString(byte* buffer)
        => MemoryMarshal.CreateReadOnlySpanFromNullTerminated(buffer).GetString();
}
