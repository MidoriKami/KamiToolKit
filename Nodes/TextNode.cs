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
        TextColor = ColorHelper.GetColor(1);
        TextOutlineColor = ColorHelper.GetColor(2);
        FontSize = 12;
        FontType = FontType.Axis;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            stringBuffer->Dtor(true);
            stringBuffer = null;
        }
        
        base.Dispose(disposing);
    }

    [JsonProperty] public Vector4 TextColor {
        get => InternalNode->TextColor.ToVector4();
        set => InternalNode->TextColor = value.ToByteColor();
    }

    [JsonProperty] public Vector4 TextOutlineColor {
        get => InternalNode->EdgeColor.ToVector4();
        set => InternalNode->EdgeColor = value.ToByteColor();
    }

    [JsonProperty]  public Vector4 BackgroundColor {
        get => InternalNode->BackgroundColor.ToVector4();
        set => InternalNode->BackgroundColor = value.ToByteColor();
    }

    public uint SelectStart {
        get => InternalNode->SelectStart;
        set => InternalNode->SelectStart = value;
    }

    public uint SelectEnd {
        get => InternalNode->SelectEnd;
        set => InternalNode->SelectEnd = value;
    }

    [JsonProperty] public AlignmentType AlignmentType {
        get => InternalNode->AlignmentType;
        set => InternalNode->SetAlignment(value);
    }

    [JsonProperty] public FontType FontType {
        get => InternalNode->FontType;
        set => InternalNode->SetFont(value);
    }

    [JsonProperty] public TextFlags TextFlags {
        get => (TextFlags) InternalNode->TextFlags;
        set => InternalNode->TextFlags = (byte) value;
    }

    [JsonProperty] public TextFlags2 TextFlags2 {
        get => (TextFlags2) InternalNode->TextFlags2;
        set => InternalNode->TextFlags2 = (byte) value;
    }

    [JsonProperty] public uint FontSize {
        get => InternalNode->FontSize;
        set => InternalNode->FontSize = (byte) value;
    }

    [JsonProperty] public uint LineSpacing {
        get => InternalNode->LineSpacing;
        set => InternalNode->LineSpacing = (byte) value;
    }
    
    [JsonProperty] public uint CharSpacing {
        get => InternalNode->CharSpacing;
        set => InternalNode->CharSpacing = (byte) value;
    }

    public uint TextId {
        get => InternalNode->TextId;
        set => InternalNode->TextId = value;
    }

    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => InternalNode->SetNumber(number, showCommas, showPlusSign, (byte) digits, zeroPad);

    public Vector2 GetTextDrawSize(SeString text) {
        using var stringContainer = new Utf8String(text.TextValue);

        ushort sizeX;
        ushort sizeY;

        InternalNode->GetTextDrawSize(&sizeX, &sizeY, stringContainer.StringPtr);
        
        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// If you want the node to resize automatically, use TextFlags.AutoAdjustNodeSize <b><em>before</em></b> setting the Text property.
    /// </summary>
    public SeString Text {
        get => InternalNode->GetText().AsDalamudSeString();
        set {
            stringBuffer->SetString(value.EncodeWithNullTerminator());
            if (stringBuffer->StringPtr.Value is not null) {
                InternalNode->SetText(stringBuffer->StringPtr);
            }
        }
    }

    [JsonProperty] public string String {
        get => Text.ToString();
        set => Text = value;
    }
}
