using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ImageNode : NodeBase<AtkImageNode> {
    protected readonly PartsList PartsList;

    public ImageNode() : base(NodeType.Image) {
        PartsList = new PartsList();

        WrapMode = 1;
        ImageNodeFlags = ImageNodeFlags.AutoFit;

        InternalNode->DrawFlags = 0x100;
        InternalNode->PartsList = PartsList.InternalPartsList;
    }
    
    protected override void Dispose(bool disposing) {
        if (disposing) {
            PartsList.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public uint PartId {
        get => InternalNode->PartId;
        set => InternalNode->PartId = (ushort) value;
    }

    public byte WrapMode {
        get => InternalNode->WrapMode;
        set => InternalNode->WrapMode = value;
    } 

    public ImageNodeFlags ImageNodeFlags {
        get => (ImageNodeFlags) InternalNode->Flags;
        set => InternalNode->Flags = (byte) value;
    }

    // The image node will take ownership of any parts added, be sure not to share parts between nodes
    public void AddPart(Part part)
        => PartsList.Add(part);
}
