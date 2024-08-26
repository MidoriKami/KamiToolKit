using System;
using ImGuiNET;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeStyles;

public class CounterNodeStyle : NodeBaseStyle {
    public int NumberWidth = 10;
    public int CommaWidth = 8;
    public int SpaceWidth = 6;
    public float CounterWidth = 32;
    public TextAlignment TextAlignment = TextAlignment.Unknown;

    public CounterStyleDisable CounterStyleDisable = CounterStyleDisable.None;
    
    public override bool DrawSettings() {
        var configChanged = base.DrawSettings();

        if (!CounterStyleDisable.HasFlag(CounterStyleDisable.NumberWidth))
            configChanged |= ImGui.SliderInt("Number Width", ref NumberWidth, 1, 20);
        
        if (!CounterStyleDisable.HasFlag(CounterStyleDisable.CommaWidth))
            configChanged |= ImGui.SliderInt("Comma Width", ref CommaWidth, 1, 20);
        
        if (!CounterStyleDisable.HasFlag(CounterStyleDisable.SpaceWidth))
            configChanged |= ImGui.SliderInt("Space Width", ref SpaceWidth, 1, 20);
        
        if (!CounterStyleDisable.HasFlag(CounterStyleDisable.CounterWidth))
            configChanged |= ImGui.SliderFloat("Counter Width", ref CounterWidth, 1, 64);
        
        if (!CounterStyleDisable.HasFlag(CounterStyleDisable.Alignment))
            configChanged |= ComboHelper.EnumCombo("Text Alignment", ref TextAlignment);
        
        return configChanged;
    }
}

[Flags]
public enum CounterStyleDisable {
    None = 0,
    NumberWidth = 1 << 1,
    CommaWidth = 1 << 2,
    SpaceWidth = 1 << 3,
    CounterWidth = 1 << 4,
    Alignment = 1 << 5,
}