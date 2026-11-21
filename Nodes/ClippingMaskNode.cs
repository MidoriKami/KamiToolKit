using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's ClippingMaskNode.
/// </summary>
public unsafe class ClippingMaskNode : NodeBase<AtkClippingMaskNode>, IClippingMaskNode {

    /// <inheritdoc />
    public PartsList PartsList { get; }

    /// <inheritdoc />
    public ushort PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

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
