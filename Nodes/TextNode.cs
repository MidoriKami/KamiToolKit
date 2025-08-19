using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class TextNode : NodeBase<AtkTextNode> {

    private Utf8String* stringBuffer = Utf8String.CreateEmpty();

    public TextNode() : base(NodeType.Text) {
        TextColor = ColorHelper.GetColor(50);
        TextOutlineColor = ColorHelper.GetColor(7);
        FontSize = 12;
        FontType = FontType.Axis;
    }

    [JsonProperty] public Vector4 TextColor {
        get => Node->TextColor.ToVector4();
        set => Node->TextColor = value.ToByteColor();
    }

    [JsonProperty] public Vector4 TextOutlineColor {
        get => Node->EdgeColor.ToVector4();
        set => Node->EdgeColor = value.ToByteColor();
    }

    [JsonProperty] public Vector4 BackgroundColor {
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

    [JsonProperty] public AlignmentType AlignmentType {
        get => Node->AlignmentType;
        set => Node->SetAlignment(value);
    }

    [JsonProperty] public FontType FontType {
        get => Node->FontType;
        set => Node->SetFont(value);
    }

    [JsonProperty] public TextFlags TextFlags {
        get => Node->TextFlags;
        set => Node->TextFlags = value;
    }

    [JsonProperty] public uint FontSize {
        get => Node->FontSize;
        set => Node->FontSize = (byte)value;
    }

    [JsonProperty] public uint LineSpacing {
        get => Node->LineSpacing;
        set => Node->LineSpacing = (byte)value;
    }

    [JsonProperty] public uint CharSpacing {
        get => Node->CharSpacing;
        set => Node->CharSpacing = (byte)value;
    }

    public uint TextId {
        get => Node->TextId;
        set => Node->TextId = value;
    }

    /// <summary>
    ///     If you want the node to resize automatically,
    ///     use TextFlags.AutoAdjustNodeSize <b><em>before</em></b>
    ///     setting the Text property.
    /// </summary>
    public SeString Text {
        get => Node->GetText().AsDalamudSeString();
        set {
            stringBuffer->SetString(value.EncodeWithNullTerminator());
            if (stringBuffer->StringPtr.Value is not null) {
                Node->SetText(stringBuffer->StringPtr);
            }
        }
    }

    [JsonProperty] public string String {
        get => Text.ToString();
        set => Text = value;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            stringBuffer->Dtor(true);
            stringBuffer = null;
        }

        base.Dispose(disposing);
    }

    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => Node->SetNumber(number, showCommas, showPlusSign, (byte)digits, zeroPad);

    public Vector2 GetTextDrawSize(SeString text) {
        using var stringContainer = new Utf8String(text.TextValue);

        ushort sizeX;
        ushort sizeY;

        Node->GetTextDrawSize(&sizeX, &sizeY, stringContainer.StringPtr);

        return new Vector2(sizeX, sizeY);
    }
}
