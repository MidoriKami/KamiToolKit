using System;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// <summary>
/// Wrapper around a AtkUldPart and AtkUldAsset, loads and holds graphics textures for display in image, and image-like nodes.
/// </summary>
public unsafe class Part : IDisposable {
    internal AtkUldPart* InternalPart;
    internal AtkUldAsset* InternalAsset;
 
    private bool customTextureLoaded;
    
    public bool IsAttached;
    private bool isDisposed;
    
    public Part() {
        InternalPart = NativeMemoryHelper.UiAlloc<AtkUldPart>();

        InternalPart->Width = 0;
        InternalPart->Height = 0;
        InternalPart->U = 0;
        InternalPart->V = 0;

        InternalAsset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();

        InternalAsset->Id = 0;
        InternalAsset->AtkTexture.Ctor();
        
        InternalPart->UldAsset = InternalAsset;
    }

    public void Dispose() {
        if (!isDisposed) {
            if (customTextureLoaded) {
                InternalAsset->AtkTexture.KernelTexture->DecRef();
                InternalAsset->AtkTexture.Destroy(false);
            }
            else {
                InternalAsset->AtkTexture.ReleaseTexture();
                InternalAsset->AtkTexture.Destroy(true);
            }

            NativeMemoryHelper.UiFree(InternalAsset);
            InternalAsset = null;

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
        get => InternalAsset->Id;
        set => InternalAsset->Id = value;
    }
    
    /// <summary>
    /// Gets the icon id of the currently loaded texture
    /// </summary>
    /// <returns>IconId or null</returns>
    public uint? GetLoadedIconId() {
        if (!InternalAsset->AtkTexture.IsTextureReady()) return null;
        if (InternalAsset->AtkTexture.Resource is null) return null;

        return InternalAsset->AtkTexture.Resource->IconId;
    }

    /// <summary>
    /// Loads a game texture via path, but processes it through the substitution provider.
    /// </summary>
    /// <param name="path">The path to the specific tex file that you want</param>
    /// <param name="provider">Dalamud ITextureSubstitutionProvider for resolving the path</param>
    public void LoadTexture(string path, ITextureSubstitutionProvider? provider = null) {
        var texturePath = path;
        
        // If we are trying to load a HR texture
        if (texturePath.Contains("_hr1")) {

            // But we are not in HR mode
            if (AtkStage.Instance()->AtkTextureResourceManager->DefaultTextureVersion is 1) {
                texturePath = texturePath.Replace("_hr1", "");
            }
        }

        if (provider is not null) {
            texturePath = provider.GetSubstitutedPath(texturePath);
        }
        
        InternalAsset->AtkTexture.LoadTexture(texturePath);
    }

    /// <summary>
    /// Release the loaded texture, decreases ref count
    /// </summary>
    public void ReleaseTexture()
        => InternalAsset->AtkTexture.ReleaseTexture();

    /// <summary>
    /// Destroys the loaded texture, with option to free the allocated memory
    /// </summary>
    /// <param name="free">If the game should free the texture in memory</param>
    public void DestroyTexture(bool free)
        => InternalAsset->AtkTexture.Destroy(free);

    /// <summary>
    /// Loads a game icon via id
    /// </summary>
    /// <param name="iconId">Icon id to load</param>
    public void LoadIcon(uint iconId)
        => InternalAsset->AtkTexture.LoadIconTexture(iconId, 0);

    /// <summary>
    /// Loads texture via an already constructed Texture*
    /// </summary>
    /// <remarks>This does not preserve any existing texture.</remarks>
    /// <param name="texture">Texture to assign to this image node.</param>
    public void LoadTexture(Texture* texture) {
        // If a texture is already loaded, dec ref it to probably free it automatically
        if (customTextureLoaded) {
            InternalAsset->AtkTexture.KernelTexture->DecRef();
        }

        InternalAsset->AtkTexture.KernelTexture = texture;
        InternalAsset->AtkTexture.TextureType = TextureType.KernelTexture;

        customTextureLoaded = true;
    }

    /// <summary>
    /// Loads a texture via dalamud texture wrap.
    /// </summary>
    /// <remarks>WIP</remarks>
    /// <param name="textureProvider">Dalamud TextureProvider that will generate KernelTexture</param>
    /// <param name="texture">Texture Wrap to convert</param>
    public void LoadTexture(ITextureProvider textureProvider, IDalamudTextureWrap texture) {
        var texturePointer = (Texture*) textureProvider.ConvertToKernelTexture(texture, true);
        LoadTexture(texturePointer);
    }
}