using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

public unsafe class ImageNode : NodeBase<AtkImageNode> {

    public readonly PartsList PartsList;

    public ImageNode() : base(NodeType.Image) {
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

    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = (ushort) value;
    }

    public WrapMode WrapMode {
        get => (WrapMode) Node->WrapMode;
        set => Node->WrapMode = (byte) value;
    }

    public ImageNodeFlags ImageNodeFlags {
        get => Node->Flags;
        set => Node->Flags = value;
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

    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);
    
    public void AddPart(params Part[] parts)
        => PartsList.Add(parts);
}
