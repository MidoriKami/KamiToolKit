using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class ClippingMaskNode : NodeBase<AtkClippingMaskNode> {
    public readonly PartsList PartsList;

    public ClippingMaskNode() : base(NodeType.ClippingMask) {
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

    public ushort PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);
    
    public void AddPart(params Part[] parts)
        => PartsList.Add(parts);
}
