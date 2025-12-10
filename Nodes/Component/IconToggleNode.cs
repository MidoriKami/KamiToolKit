using System.Numerics;

namespace KamiToolKit.Nodes;

public unsafe class IconToggleNode : SimpleComponentNode {
    private readonly IconImageNode iconNode;
    private readonly ClippingMaskNode clipNode;
    private readonly SimpleImageNode highlightNode; // For selected
    private readonly SimpleImageNode lowlightNode;  // For unselected

    public IconToggleNode() {
        iconNode = new IconImageNode {
            TextureSize = new Vector2(36, 36),
            FitTexture = true,
        };
        iconNode.AttachNode(this);

        clipNode = new SimpleClippingMaskNode {
            TextureCoordinates = Vector2.Zero,
            TextureSize = new Vector2(32, 32),
            TexturePath = "ui/uld/BgPartsMask.tex",
            Size = new Vector2(32, 32)
        };
        clipNode.AttachNode(this);

        highlightNode = new SimpleImageNode {
            Size = new Vector2(36, 36),
            IsVisible = false,
            TextureCoordinates = new Vector2(69, 1),
            TextureSize = new Vector2(36, 36),
            TexturePath = "ui/uld/BgParts.tex"
        };
        highlightNode.AttachNode(this);

        lowlightNode = new SimpleImageNode {
            Size = new Vector2(36, 36),
            IsVisible = false,
            TextureCoordinates = new Vector2(141, 1),
            TextureSize = new Vector2(36, 36),
            TexturePath = "ui/uld/BgParts.tex"
        };
        lowlightNode.AttachNode(this);
    }

    public uint IconId {
        get => iconNode.IconId;
        set => iconNode.IconId = value;
    }

    public bool IsToggled {
        get;
        set {
            field = value;
            highlightNode.IsVisible = value;
            lowlightNode.IsVisible = !value;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        // Icon is 32x32 centered within the 36x36 node
        var iconSize = Size - new Vector2(4, 4);
        var iconOffset = new Vector2(2, 2);
        iconNode.Size = iconSize;
        iconNode.Position = iconOffset;

        clipNode.Size = iconSize;
        clipNode.Position = iconOffset;

        highlightNode.Size = Size;
        highlightNode.Position = Vector2.Zero;

        lowlightNode.Size = Size;
        lowlightNode.Position = Vector2.Zero;
    }
}
