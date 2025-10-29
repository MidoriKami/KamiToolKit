using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public class ResizeNineGridNode : SimpleComponentNode {

    public readonly NineGridNode BorderNode;

    public ResizeNineGridNode() {
        BorderNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/WindowA_line.tex",
            TextureCoordinates = new Vector2(2.0f, 1.0f),
            TextureSize = new Vector2(28.0f, 3.0f),
            LeftOffset = 12,
            RightOffset = 12,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };
        BorderNode.AttachNode(this);
    }

    public bool IsHovered {
        get;
        set {
            field = value;
            if (value) {
                BorderNode.AddColor = new Vector3(100.0f, 100.0f, 100.0f) / 255.0f;
            }
            else {
                BorderNode.AddColor = Vector3.Zero;
            }
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BorderNode.Size = Size;
    }
}
