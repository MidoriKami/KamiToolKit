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
            NodeId = 3,
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(32.0f, 34.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            Size = new Vector2(16.0f, 16.0f),
            Origin = new Vector2(8.0f, 8.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = WrapMode.Tile,
        };
        UnselectedImageNode.AttachNode(this);

        SelectedImageNode = new SimpleImageNode {
            NodeId = 2,
            TexturePath = "ui/uld/ChatLog.tex",
            TextureCoordinates = new Vector2(6.0f, 34.0f),
            TextureSize = new Vector2(16.0f, 16.0f),
            Size = new Vector2(16.0f, 16.0f),
            Origin = new Vector2(8.0f, 8.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = WrapMode.Tile,
        };
        SelectedImageNode.AttachNode(this);
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
