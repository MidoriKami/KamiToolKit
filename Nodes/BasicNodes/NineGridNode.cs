using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public unsafe class NineGridNode : NodeBase<AtkNineGridNode> {
    protected readonly PartsList PartsList;
    
    public NineGridNode() : base(NodeType.NineGrid) {
        PartsList = new PartsList();

        InternalNode->PartsList = PartsList.InternalPartsList;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();
            
            base.Dispose(disposing);
        }
    }
    
    /// The image node will take ownership of any parts added, be sure not to share parts between nodes
    public void AddPart(Part part)
        => PartsList.Add(part);

    [JsonIgnore] public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = value;
    }

    [JsonIgnore] public Vector4 Offsets {
        get => new(InternalNode->TopOffset, InternalNode->BottomOffset, InternalNode->LeftOffset, InternalNode->RightOffset);
        set {
            InternalNode->TopOffset = (short)value.X;
            InternalNode->BottomOffset = (short)value.Y;
            InternalNode->LeftOffset = (short)value.Z;
            InternalNode->RightOffset = (short)value.W;
        }
    }

    [JsonIgnore] public float TopOffset {
        get => InternalNode->TopOffset;
        set => InternalNode->TopOffset = (short)value;
    }
    
    [JsonIgnore] public float BottomOffset {
        get => InternalNode->BottomOffset;
        set => InternalNode->BottomOffset = (short)value;
    }
    
    [JsonIgnore] public float LeftOffset {
        get => InternalNode->LeftOffset;
        set => InternalNode->LeftOffset = (short)value;
    }
    
    [JsonIgnore]  public float RightOffset {
        get => InternalNode->RightOffset;
        set => InternalNode->RightOffset = (short)value;
    }

    [JsonIgnore] public uint BlendMode {
        get => InternalNode->BlendMode;
        set => InternalNode->BlendMode = value;
    }

    [JsonIgnore] public byte PartsRenderType {
        get => InternalNode->PartsTypeRenderType;
        set => InternalNode->PartsTypeRenderType = value;
    }
}