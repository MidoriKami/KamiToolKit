using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games ClippingMaskNode.
/// </summary>
public unsafe class ClippingMaskNode : NodeBase<AtkClippingMaskNode> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public PartsList PartsList { get; }

    /// <summary>
    /// Gets or sets this node's PartId.
    /// </summary>
    public ushort PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    /// <summary>
    /// Adds a specified part to this node's PartsList.
    /// </summary>
    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);

    /// <summary>
    /// Adds multiple parts at once.
    /// </summary>
    public void AddPart(params Part[] parts)
        => PartsList.Add(parts);

    /// <summary>
    /// Constructs a <see cref="ClippingMaskNode"/> instance.
    /// </summary>
    public ClippingMaskNode() : base(NodeType.ClippingMask) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        if (!isNativeDestructor) {
            PartsList.Dispose();
            Node->PartsList = null;
        }

        base.Dispose(isNativeDestructor);
    }
}
