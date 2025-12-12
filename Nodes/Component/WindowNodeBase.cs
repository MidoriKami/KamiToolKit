using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public abstract unsafe class WindowNodeBase : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow> {

    protected WindowNodeBase() {
        SetInternalComponentType(ComponentType.Window);
    }
    
    public abstract Vector2 ContentSize { get; }
    public abstract Vector2 ContentStartPosition { get; }
    public abstract float HeaderHeight { get; }

    public void SetTitle(string title, string? subtitle = null)
        => Component->SetTitle(title, subtitle ?? string.Empty);
}
