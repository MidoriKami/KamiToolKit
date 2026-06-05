using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games TextNode.
/// </summary>
public unsafe class TextNode : NodeBase<AtkTextNode> {

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public Vector4 TextColor {
        get => Node->TextColor.ToVector4();
        set => Node->TextColor = value.ToByteColor();
    }

    /// <summary>
    /// Gets or sets the outline color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public Vector4 TextOutlineColor {
        get => Node->EdgeColor.ToVector4();
        set => Node->EdgeColor = value.ToByteColor();
    }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Vector4 BackgroundColor {
        get => Node->BackgroundColor.ToVector4();
        set => Node->BackgroundColor = value.ToByteColor();
    }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="BackgroundColor"/> and <see cref="SelectEnd"/>.
    /// </remarks>
    public uint SelectStart {
        get => Node->SelectStart;
        set => Node->SelectStart = value;
    }

    /// <summary>
    /// Gets or sets the selection end.
    /// </summary>
    /// <remarks>
    /// This is used in conjunction with <see cref="BackgroundColor"/> and <see cref="SelectStart"/>.
    /// </remarks>
    public uint SelectEnd {
        get => Node->SelectEnd;
        set => Node->SelectEnd = value;
    }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public AlignmentType AlignmentType {
        get => Node->AlignmentType;
        set {
            Node->SetAlignment(value);
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the used font.
    /// </summary>
    public FontType FontType {
        get => Node->FontType;
        set {
            Node->SetFont(value);
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the text flags.
    /// </summary>
    public TextFlags TextFlags {
        get => Node->TextFlags;
        set {
            Node->TextFlags = value;
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public uint FontSize {
        get => Node->FontSize;
        set {
            Node->FontSize = (byte)value;
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the vertical line spacing.
    /// </summary>
    public uint LineSpacing {
        get => Node->LineSpacing;
        set {
            Node->LineSpacing = (byte)value;
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the character spacing.
    /// </summary>
    public uint CharSpacing {
        get => Node->CharSpacing;
        set {
            Node->CharSpacing = (byte)value;
            UpdateText();
        }
    }

    /// <summary>
    /// Gets or sets the sheet type used when setting text via <see cref="TextId"/>
    /// </summary>
    public NodeData.SheetType SheetType {
        get => (NodeData.SheetType)Node->SheetType;
        set => Node->SheetType = (byte)value;
    }

    /// <summary>
    /// Gets or sets the textId, this is a row in a <see cref="NodeData.SheetType"/>.
    /// </summary>
    public uint TextId {
        get => Node->TextId;
        set => Node->TextId = value;
    }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => new(Node->GetText().AsSpan());
        set {
            using var builder = new RentedSeStringBuilder();
            Node->SetText(builder.Builder.Append(value).GetViewAsSpan());
        }
    }

    /// <summary>
    /// Gets or sets the nodes size, triggering a text update.
    /// </summary>
    public override Vector2 Size {
        get => base.Size;
        set {
            base.Size = value;
            UpdateText();
        }
    }

    /// <summary>
    /// Adds the specified text flags.
    /// </summary>
    public void AddTextFlags(params TextFlags[] flags) {
        foreach (var flag in flags) {
            TextFlags |= flag;
        }
    }

    /// <summary>
    /// Removes the specified text flags.
    /// </summary>
    public void RemoveTextFlags(params TextFlags[] flags) {
        foreach (var flag in flags) {
            TextFlags &= ~flag;
        }
    }

    /// <summary>
    /// Sets the specified number using the provided formatting params.
    /// </summary>
    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => Node->SetNumber(number, showCommas, showPlusSign, (byte)digits, zeroPad);

    /// <summary>
    /// Gets the size of the specified text if it were drawn with this nodes given params.
    /// </summary>
    public Vector2 GetTextDrawSize(ReadOnlySeString text, bool considerScale = true) {
        using var builder = new RentedSeStringBuilder();

        ushort sizeX = 0;
        ushort sizeY = 0;

        fixed (byte* ptr = builder.Builder.Append(text).GetViewAsSpan())
            Node->GetTextDrawSize(&sizeX, &sizeY, ptr, considerScale: considerScale);

        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// Gets the size of this nodes text.
    /// </summary>
    public Vector2 GetTextDrawSize(bool considerScale = true) {
        ushort sizeX = 0;
        ushort sizeY = 0;

        Node->GetTextDrawSize(&sizeX, &sizeY, considerScale: considerScale);

        return new Vector2(sizeX, sizeY);
    }

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


    private void UpdateText() {
        using var builder = new RentedSeStringBuilder();
        Node->SetText(builder.Builder.Append(String).GetViewAsSpan());
    }
}
