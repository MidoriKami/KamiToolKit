using System;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Parts;

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
            
                // Then destroy it and make it regret existing.
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
    
    public uint? GetLoadedIconId() {
        if (!InternalAsset->AtkTexture.IsTextureReady()) return null;
        if (InternalAsset->AtkTexture.Resource is null) return null;

        return InternalAsset->AtkTexture.Resource->IconId;
    }

    public void LoadTexture(string path)
        => InternalAsset->AtkTexture.LoadTexture(path);

    public void UnloadTexture()
        => InternalAsset->AtkTexture.ReleaseTexture();

    public void LoadIcon(uint iconId)
        => InternalAsset->AtkTexture.LoadIconTexture(iconId, 0);

    public void LoadImGuiTexture(IDalamudTextureWrap textureWrap) {
        // Do not attempt to load another texture, if we have one loaded already.
        if (customTextureLoaded) return;

        InternalAsset->AtkTexture.KernelTexture = Texture.CreateTexture2D(textureWrap.Width, textureWrap.Height, 3, (uint) TextureFormat.R8G8B8A8, 0, 0);

        cachedTexture = InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView;
        InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView = (void*) textureWrap.ImGuiHandle;

        InternalAsset->AtkTexture.TextureType = TextureType.KernelTexture;

        customTextureLoaded = true;
    }
}