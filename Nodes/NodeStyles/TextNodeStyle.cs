using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeStyles;

public class TextNodeStyle : NodeBaseStyle {
    public Vector4 TextColor = KnownColor.White.Vector();
    public Vector4 TextOutlineColor = KnownColor.Black.Vector();
    public Vector4 BackgroundColor;
    public AlignmentType AlignmentType;
    public FontType FontType = FontType.Axis;
    public TextFlags TextFlags;
    public TextFlags2 TextFlags2;
    public int FontSize = 12;
    public int LineSpacing;
    public int CharacterSpacing;

    public TextStyleDisable TextStyleDisable = TextStyleDisable.None;
    
    public override bool DrawSettings() {
        var configChanged = base.DrawSettings();

        if (!TextStyleDisable.HasFlag(TextStyleDisable.TextColor))
            configChanged |= ImGui.ColorEdit4("Text Color", ref TextColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.TextOutlineColor))
            configChanged |= ImGui.ColorEdit4("Text Outline Color", ref TextOutlineColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.BackgroundColor))
            configChanged |= ImGui.ColorEdit4("Background Color", ref BackgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.AlignmentType))
            configChanged |= ComboHelper.EnumCombo("Alignment Type", ref AlignmentType);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.FontType))
            configChanged |= ComboHelper.EnumCombo("Font Type", ref FontType);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.TextFlags))
            configChanged |= ComboHelper.EnumCombo("Text Flags", ref TextFlags, true);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.TextFlags2))
            configChanged |= ComboHelper.EnumCombo("Text Flags2", ref TextFlags2, true);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.FontSize))
            configChanged |= ImGui.SliderInt("Font Size", ref FontSize, 1, 64);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.LineSpacing))
            configChanged |= ImGui.SliderInt("Line Spacing", ref LineSpacing, 0, 64);
        
        if (!TextStyleDisable.HasFlag(TextStyleDisable.CharacterSpacing))
            configChanged |= ImGui.SliderInt("Character Spacing", ref CharacterSpacing, 0, 64);

        return configChanged;
    }
}

[Flags]
public enum TextStyleDisable {
    None = 0,
    TextColor = 1 << 1,
    TextOutlineColor = 1 << 2,
    BackgroundColor = 1 << 3,
    AlignmentType = 1 << 4,
    FontType = 1 << 5,
    TextFlags = 1 << 6,
    TextFlags2 = 1 << 7,
    FontSize = 1 << 8,
    LineSpacing = 1 << 9,
    CharacterSpacing = 1 << 10,
}