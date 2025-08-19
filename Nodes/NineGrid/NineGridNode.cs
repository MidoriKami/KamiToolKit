using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class NineGridNode : NodeBase<AtkNineGridNode> {

    public readonly PartsList PartsList;

    public NineGridNode() : base(NodeType.NineGrid) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
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

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();

            base.Dispose(disposing);
        }
    }

    /// The image node will take ownership of any parts added, be sure not to share parts between nodes
    public void AddPart(Part part)
        => PartsList.Add(part);
}
