using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's NineGridNode.
/// </summary>
public unsafe class NineGridNode : NodeBase<AtkNineGridNode>, INineGridNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public PartsList PartsList { get; }

    /// <inheritdoc/>
    public ICollection<Part> Parts {
        set => PartsList.Add([.. value]);
    }

    /// <inheritdoc/>
    public ushort PartId {
        get => (ushort) Node->PartId;
        set => Node->PartId = value;
    }

    /// <inheritdoc/>
    public Vector4 Offsets {
        get => new(Node->TopOffset, Node->BottomOffset, Node->LeftOffset, Node->RightOffset);
        set {
            Node->TopOffset = (short)value.X;
            Node->BottomOffset = (short)value.Y;
            Node->LeftOffset = (short)value.Z;
            Node->RightOffset = (short)value.W;
        }
    }

    /// <inheritdoc/>
    public float TopOffset {
        get => Node->TopOffset;
        set => Node->TopOffset = (short)value;
    }

    /// <inheritdoc/>
    public float BottomOffset {
        get => Node->BottomOffset;
        set => Node->BottomOffset = (short)value;
    }

    /// <inheritdoc/>
    public float LeftOffset {
        get => Node->LeftOffset;
        set => Node->LeftOffset = (short)value;
    }

    /// <inheritdoc/>
    public float RightOffset {
        get => Node->RightOffset;
        set => Node->RightOffset = (short)value;
    }

    /// <inheritdoc/>
    public uint BlendMode {
        get => Node->BlendMode;
        set => Node->BlendMode = value;
    }

    /// <inheritdoc/>
    public byte PartsRenderType {
        get => Node->PartsTypeRenderType;
        set => Node->PartsTypeRenderType = value;
    }

    /// <inheritdoc/>
    public AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);

    /// <inheritdoc/>
    public void AddPart(string path, Vector2 position, Vector2 size)
        => PartsList.Add(new Part {
            TexturePath = path,
            TextureCoordinates = position,
            Size = size,
        });

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
