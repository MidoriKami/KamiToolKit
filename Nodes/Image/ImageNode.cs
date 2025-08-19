using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ImageNode : NodeBase<AtkImageNode> {

    public readonly PartsList PartsList;

    public ImageNode() : base(NodeType.Image) {
        PartsList = new PartsList();

        WrapMode = 1;
        ImageNodeFlags = ImageNodeFlags.AutoFit;

        Node->DrawFlags = 0x100;
        Node->PartsList = PartsList.InternalPartsList;
    }

    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = (ushort)value;
    }

    public byte WrapMode {
        get => Node->WrapMode;
        set => Node->WrapMode = value;
    }

    public ImageNodeFlags ImageNodeFlags {
        get => (ImageNodeFlags)Node->Flags;
        set => Node->Flags = (byte)value;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();

            base.Dispose(disposing);
        }
    }

    public void AddPart(Part part)
        => PartsList.Add(part);
}
