using System;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Parts;

public unsafe class Asset : IDisposable {
    internal readonly AtkUldAsset* InternalAsset;

    private void* cachedTexture;
    private bool customTextureLoaded;
    
    public Asset() {
        InternalAsset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();

        InternalAsset->Id = 0;
        InternalAsset->AtkTexture.Ctor(); // not sure if this is a good idea?
    }
    
    public void Dispose() {
        if (customTextureLoaded) {
            // Restore cached texture
            InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView = cachedTexture;
            
            // Then destroy it and make it regret existing.
            InternalAsset->AtkTexture.ReleaseTexture();
            InternalAsset->AtkTexture.Destroy(true);
        }

        NativeMemoryHelper.UiFree(InternalAsset);
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
    
    public void LoadImGuiTexture(IDalamudTextureWrap texture) {
        // Do not attempt to load another texture, if we have one loaded already.
        if (customTextureLoaded) return;

        InternalAsset->AtkTexture.KernelTexture = Texture.CreateTexture2D(texture.Width, texture.Height, 3, (uint) TextureFormat.R8G8B8A8, 0, 0);

        cachedTexture = InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView;
        InternalAsset->AtkTexture.KernelTexture->D3D11ShaderResourceView = (void*) texture.ImGuiHandle;

        InternalAsset->AtkTexture.TextureType = TextureType.KernelTexture;

        customTextureLoaded = true;
    }
    
    // Some attempts at trying to load the textures properly.

    // var kernelTexture = NativeMemoryHelper.UiAlloc<Texture>();
    //
    // kernelTexture->Width = (uint) texture.Width;
    // kernelTexture->Width2 = (uint) texture.Width;
    // kernelTexture->Width3 = (uint) texture.Width;
    // kernelTexture->Height = (uint) texture.Height;
    // kernelTexture->Height2 = (uint) texture.Height;
    // kernelTexture->Height3 = (uint) texture.Height;
    // kernelTexture->MipLevel = 1;
    // kernelTexture->TextureFormat = TextureFormat.R8G8B8A8;
    // kernelTexture->D3D11ShaderResourceView = (void*) texture.ImGuiHandle;
    // kernelTexture->ArraySize = 1;
    // kernelTexture->Depth = 1;
    // kernelTexture->Flags = 0x800000;

    // internalAsset.InternalAsset->AtkTexture.KernelTexture = kernelTexture; 

    // public unsafe void DoTheThing(IDataManager dataManager) {
    //     var data = dataManager.GameData.GetFileFromDisk<TexFile>(@"D:\Downloads\huton1.tex");
    //
    //     fixed (byte* dataPtr = data.ImageData) {
    //         var newTexture = Texture.CreateTexture2D(data.TextureBuffer.Width, data.TextureBuffer.Height, data.Header.MipLevelsCount, (uint)TextureFormat.R8G8B8A8, 0u, 0u);
    //         newTexture->InitializeContents(dataPtr);
    //
    //         var imageNode = (AtkImageNode*) background.InternalResNode;
    //
    //         imageNode->PartsList->Parts->UldAsset->AtkTexture.KernelTexture = newTexture;
    //         imageNode->PartsList->Parts->UldAsset->AtkTexture.TextureType = TextureType.KernelTexture;
    //         background.Color = KnownColor.White.Vector();
    //         background.AddColor = Vector3.Zero;
    //     }
    // }

    // InternalNode->PartsList->Parts->UldAsset->AtkTexture.Resource->KernelTextureObject->D3D11ShaderResourceView = (void*)texture.ImGuiHandle;

}