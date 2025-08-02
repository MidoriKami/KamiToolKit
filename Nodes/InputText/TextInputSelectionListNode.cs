using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public class TextInputSelectionListNode : ResNode {

    public readonly NineGridNode BackgroundNode;
    public readonly TextInputButtonNode[] Buttons = new TextInputButtonNode[9];
    public readonly TextNode LabelNode;

    public TextInputSelectionListNode() {

        BackgroundNode = new SimpleNineGridNode {
            NodeId = 15,
            Size = new Vector2(186.0f, 208.0f),
            TexturePath = "ui/uld/TextInputA.tex",
            TextureCoordinates = new Vector2(48.0f, 0.0f),
            TextureSize = new Vector2(20.0f, 20.0f),
            TopOffset = 8.0f,
            BottomOffset = 8.0f,
            LeftOffset = 9.0f,
            RightOffset = 9.0f,
            PartsRenderType = 4,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.Fill | NodeFlags.EmitsEvents,
        };

        BackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            NodeId = 14,
            Position = new Vector2(13.0f, 182.0f),
            Size = new Vector2(160.0f, 21.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorRight | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            AlignmentType = (AlignmentType)21,
            FontType = FontType.MiedingerMed,
        };

        LabelNode.AttachNode(this);

        foreach (var index in Enumerable.Range(0, 9)) {
            Buttons[index] = new TextInputButtonNode {
                NodeId = (uint)(13 - index), Position = new Vector2(13.0f, 164.0f - 20.0f * index), Size = new Vector2(160.0f, 24.0f), IsVisible = true,
            };

            Buttons[index].AttachNode(this);
        }
    }
}
