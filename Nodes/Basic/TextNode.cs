using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public unsafe class TextNode : NodeBase<AtkTextNode> {

    public TextNode() : base(NodeType.Text) {
        TextColor = ColorHelper.GetColor(8);
        TextOutlineColor = ColorHelper.GetColor(7);
        FontSize = 12;
        FontType = FontType.Axis;
        LineSpacing = 12;
    }

    public Vector4 TextColor {
        get => Node->TextColor.ToVector4();
        set => Node->TextColor = value.ToByteColor();
    }

    public Vector4 TextOutlineColor {
        get => Node->EdgeColor.ToVector4();
        set => Node->EdgeColor = value.ToByteColor();
    }

    public Vector4 BackgroundColor {
        get => Node->BackgroundColor.ToVector4();
        set => Node->BackgroundColor = value.ToByteColor();
    }

    public uint SelectStart {
        get => Node->SelectStart;
        set => Node->SelectStart = value;
    }

    public uint SelectEnd {
        get => Node->SelectEnd;
        set => Node->SelectEnd = value;
    }

    public AlignmentType AlignmentType {
        get => Node->AlignmentType;
        set {
            Node->SetAlignment(value);
            UpdateText();
        }
    }

    public FontType FontType {
        get => Node->FontType;
        set {
            Node->SetFont(value);
            UpdateText();
        }
    }

    public TextFlags TextFlags {
        get => Node->TextFlags;
        set {
            Node->TextFlags = value;
            UpdateText();
        }
    }

    public uint FontSize {
        get => Node->FontSize;
        set {
            Node->FontSize = (byte)value;
            UpdateText();
        }
    }

    public uint LineSpacing {
        get => Node->LineSpacing;
        set {
            Node->LineSpacing = (byte)value;
            UpdateText();
        }
    }

    public uint CharSpacing {
        get => Node->CharSpacing;
        set {
            Node->CharSpacing = (byte)value;
            UpdateText();
        }
    }

    public uint TextId {
        get => Node->TextId;
        set => Node->TextId = value;
    }

    public ReadOnlySeString SeString {
        get => new(Node->GetText().AsSpan());
        set {
            using var builder = new RentedSeStringBuilder();
            Node->SetText(builder.Builder.Append(value).GetViewAsSpan());
        }
    }

    public string String {
        get => new ReadOnlySeStringSpan(Node->GetText().AsSpan()).ToString();
        set => Node->SetText(value);
    }

    public override Vector2 Size {
        get => base.Size;
        set {
            base.Size = value;
            UpdateText();
        }
    }

    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => Node->SetNumber(number, showCommas, showPlusSign, (byte)digits, zeroPad);

    public Vector2 GetTextDrawSize(ReadOnlySeString text) {
        using var builder = new RentedSeStringBuilder();

        ushort sizeX = 0;
        ushort sizeY = 0;

        fixed (byte* ptr = builder.Builder.Append(text).GetViewAsSpan())
            Node->GetTextDrawSize(&sizeX, &sizeY, ptr);

        return new Vector2(sizeX, sizeY);
    }

    public Vector2 GetTextDrawSize() {
        ushort sizeX = 0;
        ushort sizeY = 0;

        Node->GetTextDrawSize(&sizeX, &sizeY);

        return new Vector2(sizeX, sizeY);
    }

    private void UpdateText() {
        using var builder = new RentedSeStringBuilder();
        Node->SetText(builder.Builder.Append(SeString).GetViewAsSpan());
    }
}
