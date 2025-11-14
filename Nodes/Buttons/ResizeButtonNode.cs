using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

// Not intended for public use, this is specialized for KamiToolKit.NodeBase.Resize
internal class ResizeButtonNode : SimpleComponentNode {

    private readonly ResizeDirection resizeDirection;

    public readonly ImageNode SelectedImageNode;
    public readonly ImageNode UnselectedImageNode;

    public ResizeButtonNode(ResizeDirection direction) {
        resizeDirection = direction;

        UnselectedImageNode = new SimpleImageNode {
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(32.0f, 34.0f),
            TextureSize = new Vector2(18.0f, 18.0f),
            Size = new Vector2(16.0f, 16.0f),
            Origin = new Vector2(8.0f, 8.0f),
            NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = WrapMode.Tile,
            ImageNodeFlags = direction is ResizeDirection.BottomRight ? ImageNodeFlags.FlipV : (ImageNodeFlags)0x3,
        };
        UnselectedImageNode.AttachNode(this);

        SelectedImageNode = new SimpleImageNode {
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(4.0f, 34.0f),
            TextureSize = new Vector2(18.0f, 18.0f),
            Size = new Vector2(16.0f, 16.0f),
            Origin = new Vector2(8.0f, 8.0f),
            NodeFlags = NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = WrapMode.Tile,
            ImageNodeFlags = direction is ResizeDirection.BottomRight ? ImageNodeFlags.FlipV : (ImageNodeFlags)0x3,
        };
        SelectedImageNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        UnselectedImageNode.Size = Size - new Vector2(4.0f, 4.0f);
        UnselectedImageNode.Position = new Vector2(2.0f, 2.0f);

        SelectedImageNode.Size = Size - new Vector2(4.0f, 4.0f);
        SelectedImageNode.Position = new Vector2(2.0f, 2.0f);
    }

    public bool IsHovered {
        get;
        set {
            field = value;
            UnselectedImageNode.IsVisible = !value;
            SelectedImageNode.IsVisible = value;
        }
    }
}
