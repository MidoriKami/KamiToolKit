using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games NineGridNode.
/// </summary>
public unsafe class NineGridNode : NodeBase<AtkNineGridNode> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public PartsList PartsList { get; }

    /// <summary>
    /// Sets the collection of parts to use for this node.
    /// </summary>
    public ICollection<Part> Parts {
        set => PartsList.Add(value.ToArray());
    }

    /// <summary>
    /// Gets or sets the PartId.
    /// </summary>
    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    /// <summary>
    /// Gets or sets the texture offsets used for fitting the ninegrid texture.
    /// </summary>
    public Vector4 Offsets {
        get => new(Node->TopOffset, Node->BottomOffset, Node->LeftOffset, Node->RightOffset);
        set {
            Node->TopOffset = (short)value.X;
            Node->BottomOffset = (short)value.Y;
            Node->LeftOffset = (short)value.Z;
            Node->RightOffset = (short)value.W;
        }
    }

    /// <summary>
    /// Gets or sets the top offset for the texture.
    /// </summary>
    public float TopOffset {
        get => Node->TopOffset;
        set => Node->TopOffset = (short)value;
    }

    /// <summary>
    /// Gets or sets the bottom offset for the texture.
    /// </summary>
    public float BottomOffset {
        get => Node->BottomOffset;
        set => Node->BottomOffset = (short)value;
    }

    /// <summary>
    /// Gets or sets the left offset for the texture.
    /// </summary>
    public float LeftOffset {
        get => Node->LeftOffset;
        set => Node->LeftOffset = (short)value;
    }

    /// <summary>
    /// Gets or sets the right offset for the texture.
    /// </summary>
    public float RightOffset {
        get => Node->RightOffset;
        set => Node->RightOffset = (short)value;
    }

    /// <summary>
    /// Gets or sets the blend mode.
    /// </summary>
    public uint BlendMode {
        get => Node->BlendMode;
        set => Node->BlendMode = value;
    }

    /// <summary>
    /// Gets or sets the parts render type.
    /// </summary>
    public byte PartsRenderType {
        get => Node->PartsTypeRenderType;
        set => Node->PartsTypeRenderType = value;
    }

    /// <summary>
    /// Adds a single part.
    /// </summary>
    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);

    /// <summary>
    /// Adds multiple parts.
    /// </summary>
    public void AddPart(params Part[] parts)
        => PartsList.Add(parts);

    /// <summary>
    /// Constructs a new <see cref="NineGridNode"/>
    /// </summary>
    public NineGridNode() : base(NodeType.NineGrid) {
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
