using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public unsafe class TextNode : NodeBase<AtkTextNode> {

    private Utf8String* stringBuffer = Utf8String.CreateEmpty();

    public TextNode() : base(NodeType.Text) {
        TextColor = KnownColor.White.Vector();
        TextOutlineColor = KnownColor.Black.Vector();
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

    public Vector4 TextColor {
        get => InternalNode->TextColor.ToVector4();
        set => InternalNode->TextColor = value.ToByteColor();
    }

    public Vector4 TextOutlineColor {
        get => InternalNode->EdgeColor.ToVector4();
        set => InternalNode->EdgeColor = value.ToByteColor();
    }

    public Vector4 BackgroundColor {
        get => InternalNode->BackgroundColor.ToVector4();
        set => InternalNode->BackgroundColor = value.ToByteColor();
    }

    [JsonIgnore] public uint SelectStart {
        get => InternalNode->SelectStart;
        set => InternalNode->SelectStart = value;
    }

    [JsonIgnore] public uint SelectEnd {
        get => InternalNode->SelectEnd;
        set => InternalNode->SelectEnd = value;
    }

    public AlignmentType AlignmentType {
        get => InternalNode->AlignmentType;
        set => InternalNode->AlignmentType = value;
    }

    public FontType FontType {
        get => InternalNode->FontType;
        set => InternalNode->FontType = value;
    }

    public TextFlags TextFlags {
        get => (TextFlags) InternalNode->TextFlags;
        set => InternalNode->TextFlags = (byte) value;
    }

    public TextFlags2 TextFlags2 {
        get => (TextFlags2) InternalNode->TextFlags2;
        set => InternalNode->TextFlags2 = (byte) value;
    }

    public uint FontSize {
        get => InternalNode->FontSize;
        set => InternalNode->FontSize = (byte) value;
    }

    public uint LineSpacing {
        get => InternalNode->LineSpacing;
        set => InternalNode->LineSpacing = (byte) value;
    }
    
    public uint CharSpacing {
        get => InternalNode->CharSpacing;
        set => InternalNode->CharSpacing = (byte) value;
    }

    [JsonIgnore] public uint TextId {
        get => InternalNode->TextId;
        set => InternalNode->TextId = value;
    }

    public void SetNumber(int number, bool showCommas = false, bool showPlusSign = false, int digits = 0, bool zeroPad = false)
        => InternalNode->SetNumber(number, showCommas, showPlusSign, (byte) digits, zeroPad);

    /// <summary>
    /// If you want the node to resize automatically, use TextFlags.AutoAdjustNodeSize <b><em>before</em></b> setting the Text property.
    /// </summary>
    [JsonIgnore] public SeString Text {
        get => InternalNode->GetText().AsDalamudSeString();
        set {
            stringBuffer->SetString(value.EncodeWithNullTerminator());
            if (stringBuffer->StringPtr.Value is not null) {
                InternalNode->SetText(stringBuffer->StringPtr);
            }
        }
    }

    public string String {
        get => Text.ToString();
        set => Text = value;
    }

    public override void DrawConfig() {
        base.DrawConfig();

        using var textNode = ImRaii.TreeNode("Text Node");
        if (!textNode) return;
        
        using var table = ImRaii.Table("basic_property_table", 2);
        if (!table) return;
		
        ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
        ImGui.TableSetupColumn("##configuration", ImGuiTableColumnFlags.WidthStretch, 2.0f);

        ImGui.TableNextRow();
        
		ImGui.TableNextColumn();
        ImGui.Text("Text Color");

        ImGui.TableNextColumn();
        var textColor = TextColor;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4("##TextColor", ref textColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
            TextColor = textColor;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Outline Color");

        ImGui.TableNextColumn();
        var outlineColor = TextOutlineColor;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4("##TextOutlineColor", ref outlineColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
            TextOutlineColor = outlineColor;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Background Color");

        ImGui.TableNextColumn();
        var backgroundColor = BackgroundColor;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4("##TextBackgroundColor", ref backgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
            BackgroundColor = backgroundColor;
        }

        ImGui.Spacing();
        
        ImGui.TableNextColumn();
        ImGui.Text("Font Size");
        
        ImGui.TableNextColumn();
        var fontSize = (int)FontSize;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##FontSize", ref fontSize, 0, 0)) {
            FontSize = (uint)fontSize;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Line Spacing");
        
        ImGui.TableNextColumn();
        var lineSpacing = (int)LineSpacing;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##LineSpacing", ref lineSpacing, 0, 0)) {
            LineSpacing = (uint)lineSpacing;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Char Spacing");
        
        ImGui.TableNextColumn();
        var charSpacing = (int)CharSpacing;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##CharSpacing", ref charSpacing, 0, 0)) {
            CharSpacing = (uint)charSpacing;
        }
        
        ImGui.Spacing();
        
        ImGui.TableNextColumn();
        ImGui.Text("Text");
        
        ImGui.TableNextColumn();
        var text = String;
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.InputTextMultiline("##Text", ref text, 2000, ImGui.GetContentRegionAvail() with { Y = ImGuiHelpers.GetButtonSize("A").Y * 2.0f } ,ImGuiInputTextFlags.AutoSelectAll)) {
            String = text;
        }
        
        ImGui.Spacing();
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Alignment");
        
        ImGui.TableNextColumn();
        var alignmentType = AlignmentType;
        if (ComboHelper.EnumCombo("##AlignmentType", ref alignmentType)) {
            AlignmentType = alignmentType;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Font Type");
        
        ImGui.TableNextColumn();
        var fontType = FontType;
        if (ComboHelper.EnumCombo("##FontType", ref fontType)) {
            FontType = fontType;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Flags");
        
        ImGui.TableNextColumn();
        var textFlags = TextFlags;
        if (ComboHelper.EnumCombo("##TextFlags", ref textFlags, true)) {
            TextFlags = textFlags;
        }
        
        ImGui.TableNextColumn();
        ImGui.Text("Text Flags 2");
        
        ImGui.TableNextColumn();
        var textFlags2 = TextFlags2;
        if (ComboHelper.EnumCombo("##TextFlags2", ref textFlags2, true)) {
            TextFlags2 = textFlags2;
        }
    }
}
