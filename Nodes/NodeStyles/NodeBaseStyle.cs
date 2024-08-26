using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.NodeStyles;

public class NodeBaseStyle {
    public Vector2 Position;
    public Vector2 Size;
    public Vector2 Scale = Vector2.One;
    public bool IsVisible = true;
    public NodeFlags NodeFlags = NodeFlags.Visible;
    public Vector4 Color = Vector4.One;
    public Vector3 AddColor = Vector3.Zero;
    public Vector3 MultiplyColor = Vector3.One * 100.0f / 255.0f;
    public Vector4 Margin;

    public BaseStyleDisable BaseDisable = BaseStyleDisable.None;

    public virtual bool DrawSettings() {
        var configChanged = false;
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Visible))
            configChanged |= ImGui.Checkbox("Visible", ref IsVisible);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Position))
            configChanged |= ImGui.DragFloat2("Position", ref Position, 5.0f);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Size))
            configChanged |= ImGui.DragFloat2("Size", ref Size);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Scale))
            configChanged |= ImGui.DragFloat2("Scale", ref Scale, 0.005f);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.NodeFlags))
            configChanged |= ComboHelper.EnumCombo("Node Flags", ref NodeFlags, true);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Color))
            configChanged |= ImGui.ColorEdit4("Color", ref Color, ImGuiColorEditFlags.AlphaPreviewHalf);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.AddColor))
            configChanged |= ImGui.ColorEdit3("Add Color", ref AddColor);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.MultiplyColor))
            configChanged |= ImGui.ColorEdit3("Multiply Color", ref MultiplyColor);
        
        if (!BaseDisable.HasFlag(BaseStyleDisable.Margin))
            configChanged |= ImGui.DragFloat4("Margin", ref Margin);

        return configChanged;
    }
}

[Flags]
public enum BaseStyleDisable {
    None = 0,
    Visible = 1 << 1,
    Position = 1 << 2,
    Size = 1 << 3,
    Scale = 1 << 4,
    NodeFlags = 1 << 5,
    Color = 1 << 6,
    AddColor = 1 << 7,
    MultiplyColor = 1 << 8,
    Margin = 1 << 9,
}