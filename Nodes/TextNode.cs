using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games TextNode.
/// </summary>
public unsafe class TextNode : NodeBase<AtkTextNode>, ITextNode {

    /// <inheritdoc />
    public Vector4 TextColor {
        get => Node->TextColor.ToVector4();
        set => Node->TextColor = value.ToByteColor();
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => Node->EdgeColor.ToVector4();
        set => Node->EdgeColor = value.ToByteColor();
    }

    /// <inheritdoc />
    public Vector4 BackgroundColor {
        get => Node->BackgroundColor.ToVector4();
        set => Node->BackgroundColor = value.ToByteColor();
    }

    /// <inheritdoc />
    public uint SelectStart {
        get => Node->SelectStart;
        set => Node->SelectStart = value;
    }

    /// <inheritdoc />
    public uint SelectEnd {
        get => Node->SelectEnd;
        set => Node->SelectEnd = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => Node->AlignmentType;
        set {
            if (Node->AlignmentType == value) return;

            Node->SetAlignment(value);
            UpdateText();
        }
    }

    /// <inheritdoc />
    public FontType FontType {
        get => Node->FontType;
        set {
            if (Node->FontType == value) return;

            Node->SetFont(value);
            UpdateText();
        }
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => Node->TextFlags;
        set {
            if (Node->TextFlags == value) return;

            Node->TextFlags = value;
            UpdateText();
        }
    }

    /// <inheritdoc />
    public uint FontSize {
        get => Node->FontSize;
        set {
            if (Node->FontSize == value) return;

            Node->FontSize = (byte)value;
            UpdateText();
        }
    }

    /// <inheritdoc />
    public uint LineSpacing {
        get => Node->LineSpacing;
        set {
            if (Node->LineSpacing == value) return;

            Node->LineSpacing = (byte)value;
            UpdateText();
        }
    }

    /// <inheritdoc />
    public uint CharSpacing {
        get => Node->CharSpacing;
        set {
            if (Node->CharSpacing == value) return;

            Node->CharSpacing = (byte)value;
            UpdateText();
        }
    }

    /// <inheritdoc />
    public NodeData.SheetType SheetType {
        get => (NodeData.SheetType)Node->SheetType;
        set => Node->SheetType = (byte)value;
    }

    /// <inheritdoc />
    public uint TextId {
        get => Node->TextId;
        set => Node->TextId = value;
    }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => new(Node->GetText().AsSpan());
        set {
            using var builder = new RentedSeStringBuilder();
            Node->SetText(builder.Append(value).GetViewAsSpan());
        }
    }

    /// <inheritdoc />
    public override Vector2 Size {
        get => base.Size;
        set {
            if (base.Size == value) return;

            base.Size = value;
            UpdateText();
        }
    }

    /// <inheritdoc />
    public void AddTextFlags(params TextFlags[] flags) {
        foreach (var flag in flags) {
            TextFlags |= flag;
        }
    }

    /// <inheritdoc />
    public void RemoveTextFlags(params TextFlags[] flags) {
        foreach (var flag in flags) {
            TextFlags &= ~flag;
        }
    }

    /// <inheritdoc />
    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => Node->SetNumber(number, showCommas, showPlusSign, (byte)digits, zeroPad);

    /// <inheritdoc />
    public Vector2 GetTextDrawSize(ReadOnlySeString text, bool considerScale = true) {
        using var builder = new RentedSeStringBuilder();

        ushort sizeX = 0;
        ushort sizeY = 0;

        fixed (byte* ptr = builder.Append(text).GetViewAsSpan())
            Node->GetTextDrawSize(&sizeX, &sizeY, ptr, considerScale: considerScale);

        return new Vector2(sizeX, sizeY);
    }

    /// <inheritdoc />
    public Vector2 GetTextDrawSize(bool considerScale = true) {
        ushort sizeX = 0;
        ushort sizeY = 0;

        Node->GetTextDrawSize(&sizeX, &sizeY, considerScale: considerScale);

        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// Constructs a new <see cref="TextNode"/>
    /// </summary>
    /// <remarks>
    /// This will default various properties to standard values
    /// and colors for the theme that was active when this was constructed.
    /// </remarks>
    public TextNode() : base(NodeType.Text) {
        TextColor = ColorHelper.GetColor(8);
        TextOutlineColor = ColorHelper.GetColor(7);
        FontSize = 12;
        FontType = FontType.Axis;
        LineSpacing = 12;
        AlignmentType = AlignmentType.Left;

        if (AtkStage.Instance()->AtkUIColorHolder->ActiveColorThemeType is 0) {
            AddTextFlags(TextFlags.Emboss);
        }
    }

    /// <summary>
    /// Re-invokes SetText with the currently set <see cref="String"/> to trigger flag size and position updates.
    /// </summary>
    private void UpdateText() {
        using var builder = new RentedSeStringBuilder();
        Node->SetText(builder.Append(String).GetViewAsSpan());
    }
}
