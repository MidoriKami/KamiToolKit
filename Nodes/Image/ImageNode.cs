using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ImageNode : NodeBase<AtkImageNode> {

    public readonly PartsList PartsList;

    public ImageNode() : base(NodeType.Image) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
    }

    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = (ushort)value;
    }

    public WrapMode WrapMode {
        get => (WrapMode) Node->WrapMode;
        set => Node->WrapMode = (byte) value;
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
    
    /// <summary>
    ///     When set to true, will cause the loaded texture to
    ///     fit itself to the size of the node
    /// </summary>
    public bool FitTexture {
        set {
            if (value) {
                ImageNodeFlags = ImageNodeFlags.AutoFit;
                WrapMode = WrapMode.Stretch;
            }
        }
    }

    public void AddPart(Part part)
        => PartsList.Add(part);
}
