using System;
using System.Numerics;
using ImGuiNET;

namespace KamiToolKit.Nodes.NodeStyles;

public class ProgressBarNodeStyle : NodeBaseStyle {
    public Vector4 BackgroundColor;
    public Vector4 BarColor;

    public ProgressBarStyleDisable ProgressBarStyleDisable = ProgressBarStyleDisable.None;
    
    public override bool DrawSettings() {
        var configChanged = base.DrawSettings();

        if (!ProgressBarStyleDisable.HasFlag(ProgressBarStyleDisable.BackgroundColor))
            configChanged |= ImGui.ColorEdit4("Bar Background Color", ref BackgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf);
        
        if (!ProgressBarStyleDisable.HasFlag(ProgressBarStyleDisable.BarColor))
            configChanged |= ImGui.ColorEdit4("Bar Color", ref BarColor, ImGuiColorEditFlags.AlphaPreviewHalf);

        return configChanged;
    }
}

[Flags]
public enum ProgressBarStyleDisable {
    None = 0,
    BackgroundColor = 1 << 1,
    BarColor = 1 << 2,
}