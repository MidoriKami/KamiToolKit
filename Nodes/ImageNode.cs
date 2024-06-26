using System.Numerics;
using Dalamud.Interface.Internal;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class ImageNode : NodeBase<AtkImageNode> {

    public ImageNode() : base(NodeType.Image) {
        var asset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();
        asset->Id = 1;
        asset->AtkTexture.Ctor();

        var part = NativeMemoryHelper.UiAlloc<AtkUldPart>();
        part->UldAsset = asset;
        part->U = 0;
        part->V= 0;
        part->Height = 0;
        part->Width = 0;

        var partsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();
        partsList->Parts = part;
        partsList->Id = 1;
        partsList->PartCount = 1;

        WrapMode = WrapMode.Unknown;
        ImageNodeFlags = ImageNodeFlags.AutoFit;

        InternalNode->DrawFlags = 0x100;
        InternalNode->PartsList = partsList;
    }
    
    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalNode->UnloadTexture();
            
            NativeMemoryHelper.UiFree(InternalNode->PartsList->Parts->UldAsset);
            NativeMemoryHelper.UiFree(InternalNode->PartsList->Parts);
            NativeMemoryHelper.UiFree(InternalNode->PartsList);
            
            base.Dispose(disposing);
        }
    }
    
    public float U {
        get => InternalNode->PartsList->Parts->U;
        set => InternalNode->PartsList->Parts->U = (ushort) value;
    }
    
    public float V {
        get => InternalNode->PartsList->Parts->V;
        set => InternalNode->PartsList->Parts->V = (ushort) value;
    }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public float TextureHeight {
        get => InternalNode->PartsList->Parts->Height;
        set => InternalNode->PartsList->Parts->Height = (ushort) value;
    }
    
    public float TextureWidth {
        get => InternalNode->PartsList->Parts->Width;
        set => InternalNode->PartsList->Parts->Width = (ushort) value;
    }

    public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }
    
    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = (ushort) value;
    }

    public WrapMode WrapMode {
        get => (WrapMode)InternalNode->WrapMode;
        set => InternalNode->WrapMode = (byte) value;
    } 

    public ImageNodeFlags ImageNodeFlags {
        get => (ImageNodeFlags) InternalNode->Flags;
        set => InternalNode->Flags = (byte) value;
    }

    public void LoadTexture(string path)
        => InternalNode->LoadTexture(path);

    public void UnloadTexture()
        => InternalNode->UnloadTexture();

    public void LoadIcon(uint iconId)
        => InternalNode->LoadIconTexture(iconId, 0);

    /// <summary>
    /// Don't use, experimental. Will crash your game.
    /// </summary>
    /// <param name="texture"></param>
    public void SetImGuiTexture(IDalamudTextureWrap texture) {
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
}

public enum WrapMode {
    Unknown = 1,
}