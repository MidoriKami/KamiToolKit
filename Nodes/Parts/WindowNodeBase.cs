using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract base class implementation of a WindowNode and its associated component.
/// This one is intended for external use!
/// </summary>
public abstract unsafe class WindowNodeBase : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow>, IWindowNode {

    /// <inheritdoc />
    public abstract ReadOnlySeString Title { get; set; }

    /// <inheritdoc />
    public abstract ReadOnlySeString Subtitle { get; set; }

    /// <inheritdoc />
    public abstract bool ShowCloseButton { get; set; }

    /// <inheritdoc />
    public abstract bool ShowConfigButton { get; set; }

    /// <inheritdoc />
    public abstract bool ShowHelpButton { get; set; }

    /// <inheritdoc />
    public abstract bool Focused { get; set; }

    /// <inheritdoc />
    public abstract Vector2 ContentSize { get; }

    /// <inheritdoc />
    public abstract Vector2 ContentStartPosition { get; }

    /// <inheritdoc />
    public abstract float HeaderHeight { get; }

    /// <inheritdoc />
    public abstract ResNode WindowHeaderFocusNode { get; }

    /// <inheritdoc />
    public virtual void SetTitle(string title, string? subtitle = null)
        => Component->SetTitle(title, subtitle ?? string.Empty);

    /// <summary>
    /// Constructs a new <see cref="WindowNodeBase"/>
    /// </summary>
    protected WindowNodeBase()
        => SetInternalComponentType(ComponentType.Window);
}
