using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class NineGridNode : NodeBase<AtkNineGridNode> {

    public readonly PartsList PartsList;

    public NineGridNode() : base(NodeType.NineGrid) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (!isNativeDestructor) {
                PartsList.Dispose();
                Node->PartsList = null;
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    public ICollection<Part> Parts {
        set => PartsList.Add(value.ToArray());
    }

    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    public Vector4 Offsets {
        get => new(Node->TopOffset, Node->BottomOffset, Node->LeftOffset, Node->RightOffset);
        set {
            Node->TopOffset = (short)value.X;
            Node->BottomOffset = (short)value.Y;
            Node->LeftOffset = (short)value.Z;
            Node->RightOffset = (short)value.W;
        }
    }

    public float TopOffset {
        get => Node->TopOffset;
        set => Node->TopOffset = (short)value;
    }

    public float BottomOffset {
        get => Node->BottomOffset;
        set => Node->BottomOffset = (short)value;
    }

    public float LeftOffset {
        get => Node->LeftOffset;
        set => Node->LeftOffset = (short)value;
    }

    public float RightOffset {
        get => Node->RightOffset;
        set => Node->RightOffset = (short)value;
    }

    public uint BlendMode {
        get => Node->BlendMode;
        set => Node->BlendMode = value;
    }

    public byte PartsRenderType {
        get => Node->PartsTypeRenderType;
        set => Node->PartsTypeRenderType = value;
    }

    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);
    
    public void AddPart(params Part[] parts)
        => PartsList.Add(parts);
}
