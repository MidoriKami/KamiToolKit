using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games ImageNode.
/// </summary>
public unsafe class ImageNode : NodeBase<AtkImageNode> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public PartsList PartsList { get; }

    /// <summary>
    /// Gets or sets the PartId.
    /// </summary>
    public uint PartId {
        get => Node->PartId;
        set => Node->PartId = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the wrap mode for the displayed image.
    /// </summary>
    public WrapMode WrapMode {
        get => (WrapMode)Node->WrapMode;
        set => Node->WrapMode = (byte)value;
    }

    /// <summary>
    /// Gets or sets the image node flags.
    /// </summary>
    public ImageNodeFlags ImageNodeFlags {
        get => Node->Flags;
        set => Node->Flags = value;
    }

    /// <summary>
    /// When set to true, will cause the loaded texture to
    /// fit itself to the size of the node
    /// </summary>
    /// <remarks>
    /// Sets AutoFit ImageNodeFlag and Stretch WrapMode
    /// </remarks>
    public bool FitTexture {
        set {
            if (value) {
                ImageNodeFlags = ImageNodeFlags.AutoFit;
                WrapMode = WrapMode.Stretch;
            }
        }
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
    /// Constructs a new <see cref="IconImageNode"/>
    /// </summary>
    public ImageNode() : base(NodeType.Image) {
        PartsList = new PartsList();

        Node->PartsList = PartsList.InternalPartsList;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing && !IsDisposed) {
            if (!isNativeDestructor) {
                PartsList.Dispose();
                Node->PartsList = null;
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }
}
