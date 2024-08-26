using System;
using System.Numerics;
using ImGuiNET;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeStyles;

public class ListNodeStyle : NodeBaseStyle {
    public LayoutAnchor LayoutAnchor;
    public bool FitContents;
    public bool ClipContents;
    public LayoutOrientation LayoutOrientation;
    public Vector4 BackgroundColor;
    public bool BackgroundVisible;
    public bool BorderVisible;

    public ListStyleDisable ListStyleDisable = ListStyleDisable.None;
    
    public override bool DrawSettings() {
        var configChanged = base.DrawSettings();

        if (!ListStyleDisable.HasFlag(ListStyleDisable.Anchor))
            configChanged |= ComboHelper.EnumCombo("Anchor", ref LayoutAnchor);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.Orientation))
            configChanged |= ComboHelper.EnumCombo("Orientation", ref LayoutOrientation);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.BackgroundColor))
            configChanged |= ImGui.ColorEdit4("Background Color", ref BackgroundColor);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.FitContents))
            configChanged |= ImGui.Checkbox("Fit Contents", ref FitContents);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.ClipContents))
            configChanged |= ImGui.Checkbox("Clip Contents", ref ClipContents);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.Background))
            configChanged |= ImGui.Checkbox("Show Background", ref BackgroundVisible);
        
        if (!ListStyleDisable.HasFlag(ListStyleDisable.Border))
            configChanged |= ImGui.Checkbox("Show Border", ref BorderVisible);
        
        return configChanged;
    }
}

[Flags]
public enum ListStyleDisable {
    None = 0,
    Anchor = 1 << 0,
    Orientation = 1 << 1,
    BackgroundColor = 1 << 2, 
    FitContents = 1 << 3,
    ClipContents = 1 << 4,
    Background = 1 << 5,
    Border = 1 << 6,
}