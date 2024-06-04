using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

// Untested, this node might not work at all.
public unsafe class NineGridNode() : NodeBase<AtkNineGridNode>(NodeType.NineGrid) {
    public AtkUldPartsList* PartsList {
        get => InternalNode->PartsList;
        set => InternalNode->PartsList = value;
    }

    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
    }

    public Vector4 Offsets {
        get => new Vector4(InternalNode->TopOffset, InternalNode->BottomOffset, InternalNode->LeftOffset, InternalNode->RightOffset);
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
}

public enum PartsRenderType {
    PartsType = 1 << 1,
    RenderType = 1 << 2,
}