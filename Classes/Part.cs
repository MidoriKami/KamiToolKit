using System;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Parts;

/// <summary>
/// Wrapper around a AtkUldPart and AtkUldAsset, loads and holds graphics textures for display in image, and image-like nodes.
/// </summary>
public unsafe class Part : IDisposable {
    internal AtkUldPart* InternalPart;
    internal readonly AtkUldAsset* InternalAsset;
 
    private void* cachedTexture;
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
                // Restore cached texture
                InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView = cachedTexture;
            
                // Then destroy it and make it regret existing
                InternalAsset->AtkTexture.ReleaseTexture();
                InternalAsset->AtkTexture.Destroy(true);
            }

            NativeMemoryHelper.UiFree(InternalAsset);

            if (!IsAttached) {
                NativeMemoryHelper.UiFree(InternalPart);
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
    /// Loads a game texture via path
    /// </summary>
    /// <example1>"ui/icon/065000/65108_hr1.tex"</example1>
    /// <example2>"ui/uld/ActionBar_hr1.tex"</example2>
    /// <param name="path">Path to native game resource</param>
    public void LoadTexture(string path) {
        var texturePath = path;
            
        // If we are trying to load a HR texture
        if (texturePath.Contains("_hr1")) {
                
            // But we are not in HR mode
            if (AtkStage.Instance()->AtkTextureResourceManager->DefaultTextureVersion is 1) {
                texturePath = texturePath.Replace("_hr1", "");
            }
        }
            
        InternalAsset->AtkTexture.LoadTexture(texturePath);
    }

    /// <summary>
    /// Loads a game texture via path, but processes it through the substitution provider.
    /// </summary>
    /// <param name="path">The path to the specific tex file that you want</param>
    /// <param name="provider">Dalamud ITextureSubstitutionProvider for resolving the path</param>
    public void LoadTexture(string path, ITextureSubstitutionProvider provider) {
        var texturePath = path;
        
        // If we are trying to load a HR texture
        if (texturePath.Contains("_hr1")) {

            // But we are not in HR mode
            if (AtkStage.Instance()->AtkTextureResourceManager->DefaultTextureVersion is 1) {
                texturePath = texturePath.Replace("_hr1", "");
            }
        }
        
        var substitutionPath = provider.GetSubstitutedPath(texturePath);
        
        InternalAsset->AtkTexture.LoadTexture(substitutionPath);
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
    /// Load a DalamudTextureWrap texture into this node.
    /// </summary>
    /// <remarks>
    /// This does not transfer ownership of the texture to the node, you are required
    /// to keep the texture alive during the lifetime of this node.</remarks>
    /// <param name="textureWrap">Dalamud Texture to Load</param>
    public void LoadImGuiTexture(IDalamudTextureWrap textureWrap) {
        // Do not attempt to load another texture, if we have one loaded already.
        if (customTextureLoaded) return;

        InternalAsset->AtkTexture.KernelTexture = Texture.CreateTexture2D(textureWrap.Width, textureWrap.Height, 3, TextureFormat.B8G8R8A8_UNORM, 0, 0);

        cachedTexture = InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView;
        InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView = (void*) textureWrap.ImGuiHandle;

        InternalAsset->AtkTexture.TextureType = TextureType.KernelTexture;

        customTextureLoaded = true;
    }
}