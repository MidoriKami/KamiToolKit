using System;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

/// <summary>
/// Wrapper around a AtkUldPart and AtkUldAsset, loads and holds graphics textures for display in image, and image-like nodes.
/// </summary>
public unsafe class Part : IDisposable {
    internal AtkUldPart* InternalPart;
    private AtkUldAsset* internalAsset;
 
    private bool customTextureLoaded;
    
    public bool IsAttached;
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

    public void Dispose() {
        if (!isDisposed) {
            if (customTextureLoaded) {
                internalAsset->AtkTexture.KernelTexture->DecRef();
                internalAsset->AtkTexture.Destroy(false);
            }
            else {
                // todo: eventually reevaluate texture refcounting

                // internalAsset->AtkTexture.ReleaseTexture();
                // internalAsset->AtkTexture.Destroy(true);
            }

            NativeMemoryHelper.UiFree(internalAsset);
            internalAsset = null;

            if (!IsAttached) {
                NativeMemoryHelper.UiFree(InternalPart);
                InternalPart = null;
            }
        }

        isDisposed = true;
    }

    public float Width {
        get => InternalPart->Width;
        set => InternalPart->Width = (ushort) value;
    }

    public float Height {
        get => InternalPart->Height;
        set => InternalPart->Height = (ushort) value;
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
        set => InternalPart->U = (ushort) value;
    }

    public float V {
        get => InternalPart->V;
        set => InternalPart->V = (ushort) value;
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
    
    /// <summary>
    /// Gets the icon id of the currently loaded texture
    /// </summary>
    /// <returns>IconId or null</returns>
    public uint? GetLoadedIconId() {
        if (!internalAsset->AtkTexture.IsTextureReady()) return null;
        if (internalAsset->AtkTexture.Resource is null) return null;

        return internalAsset->AtkTexture.Resource->IconId;
    }

    /// <summary>
    /// Gets the loaded tex file resource handle texture, string.Empty if null
    /// </summary>
    /// <returns></returns>
    public string GetLoadedPath() {
        if (internalAsset is null) return string.Empty;
        if (internalAsset->AtkTexture.Resource is null) return string.Empty;
        if (internalAsset->AtkTexture.Resource->TexFileResourceHandle is null) return string.Empty;
        
        return internalAsset->AtkTexture.Resource->TexFileResourceHandle->FileName.ToString();
    }

    internal static int TextureVersion() 
        => AtkStage.Instance()->AtkTextureResourceManager->DefaultTextureVersion;

    /// <summary>
    /// Load the specified path, this will try to select the correct path based on theme
    /// and resolve through any texture substitutions defined by other plugins.
    /// </summary>
    /// <param name="path">Path to load</param>
    public void LoadTexture(string path) {
        var texturePath = path.Replace("_hr1", string.Empty);

        var themedPath = texturePath.Replace("uld", GetThemePathModifier());
        if (DalamudInterface.Instance.DataManager.FileExists(themedPath)) {
            texturePath = themedPath;
        }
        
        internalAsset->AtkTexture.LoadTexture(texturePath, TextureVersion());
    }

    private string GetThemePathModifier() => AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType switch {
        1 => "uld/light",
        2 => "uld/third",
        3 => "uld/fourth",
        _ => "uld",
    };

    /// <summary>
    /// Release the loaded texture, decreases ref count
    /// </summary>
    public void ReleaseTexture()
        => internalAsset->AtkTexture.ReleaseTexture();

    /// <summary>
    /// Destroys the loaded texture, with option to free the allocated memory
    /// </summary>
    /// <param name="free">If the game should free the texture in memory</param>
    public void DestroyTexture(bool free)
        => internalAsset->AtkTexture.Destroy(free);

    /// <summary>
    /// Loads a game icon via id
    /// </summary>
    /// <param name="iconId">Icon id to load</param>
    public void LoadIcon(uint iconId)
        => internalAsset->AtkTexture.LoadIconTexture(iconId, 0);

    /// <summary>
    /// Loads texture via an already constructed Texture*
    /// </summary>
    /// <remarks>This does not preserve any existing texture.</remarks>
    /// <param name="texture">Texture to assign to this image node.</param>
    public void LoadTexture(Texture* texture) {
        // If a texture is already loaded, dec ref it to probably free it automatically
        if (customTextureLoaded) {
            internalAsset->AtkTexture.KernelTexture->DecRef();
        }

        internalAsset->AtkTexture.KernelTexture = texture;
        internalAsset->AtkTexture.TextureType = TextureType.KernelTexture;

        customTextureLoaded = true;
    }

    /// <summary>
    /// Loads a texture via dalamud texture wrap.
    /// </summary>
    /// <remarks>WIP</remarks>
    /// <param name="textureProvider">Dalamud TextureProvider that will generate KernelTexture</param>
    /// <param name="texture">Texture Wrap to convert</param>
    public void LoadTexture(IDalamudTextureWrap texture) {
        var texturePointer = (Texture*) DalamudInterface.Instance.TextureProvider.ConvertToKernelTexture(texture, true);
        LoadTexture(texturePointer);
    }
}