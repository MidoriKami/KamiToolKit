using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.NodeBase;

namespace KamiToolKit.Nodes;

public unsafe class NineGridNode : NodeBase<AtkNineGridNode> {
    public NineGridNode() : base(NodeType.NineGrid) {
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

        InternalNode->PartsList = partsList;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            InternalNode->PartsList->Parts->UldAsset->AtkTexture.ReleaseTexture();
            
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
    
    public AtkUldPartsList* PartsList {
        get => InternalNode->PartsList;
        set => InternalNode->PartsList = value;
    }

    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
    }

    public Vector4 Offsets {
        get => new(InternalNode->TopOffset, InternalNode->BottomOffset, InternalNode->LeftOffset, InternalNode->RightOffset);
        set {
            InternalNode->TopOffset = (short)value.X;
            InternalNode->BottomOffset = (short)value.Y;
            InternalNode->LeftOffset = (short)value.Z;
            InternalNode->RightOffset = (short)value.W;
        }
    }

    public float TopOffset {
        get => InternalNode->TopOffset;
        set => InternalNode->TopOffset = (short)value;
    }
    
    public float BottomOffset {
        get => InternalNode->BottomOffset;
        set => InternalNode->BottomOffset = (short)value;
    }
    
    public float LeftOffset {
        get => InternalNode->LeftOffset;
        set => InternalNode->LeftOffset = (short)value;
    }
    
    public float RightOffset {
        get => InternalNode->RightOffset;
        set => InternalNode->RightOffset = (short)value;
    }

    public uint BlendMode {
        get => InternalNode->BlendMode;
        set => InternalNode->BlendMode = value;
    }

    public PartsRenderType PartsRenderType {
        get => (PartsRenderType) InternalNode->PartsTypeRenderType;
        set => InternalNode->PartsTypeRenderType = (byte) value;
    }

    public void LoadTexture(string path)
        => PartsList->Parts->UldAsset->AtkTexture.LoadTexture(path);
}

public enum PartsRenderType {
    PartsType = 1 << 1,
    RenderType = 1 << 2,
}