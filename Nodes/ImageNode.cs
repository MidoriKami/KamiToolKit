using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's ImageNode.
/// </summary>
public unsafe class ImageNode : NodeBase<AtkImageNode>, IImageNode {

    /// <inheritdoc />
    public PartsList PartsList { get; }

    /// <inheritdoc />
    public ushort PartId {
        get => Node->PartId;
        set => Node->PartId = value;
    }

    /// <inheritdoc />
    public WrapMode WrapMode {
        get => (WrapMode)Node->WrapMode;
        set => Node->WrapMode = (byte)value;
    }

    /// <inheritdoc />
    public ImageNodeFlags ImageNodeFlags {
        get => Node->Flags;
        set => Node->Flags = value;
    }

    /// <inheritdoc />
    public bool FitTexture {
        set {
            if (value) {
                ImageNodeFlags = ImageNodeFlags.AutoFit;
                WrapMode = WrapMode.Stretch;
            }
        }
    }

    /// <summary>
    /// Constructs a new <see cref="ImageNode"/>
    /// </summary>
    public ImageNode() : base(NodeType.Image) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
    }

    /// <inheritdoc />
    public AtkUldPart* AddPart(Part part) {
        return PartsList.Add(part);
    }

    /// <inheritdoc />
    public void AddPart(params Part[] parts) {
        PartsList.Add(parts);
    }

    /// <inheritdoc />
    public void AddPart(string path, Vector2 position, Vector2 size) {
        PartsList.Add(new Part {
            TexturePath = path,
            TextureCoordinates = position,
            Size = size,
        });
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
