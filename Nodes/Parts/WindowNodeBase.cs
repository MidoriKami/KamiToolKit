using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract base class implementation of a WindowNode and its associated component.
/// This one is intended for external use!
/// </summary>
public abstract unsafe class WindowNodeBase : ComponentNode<AtkComponentWindow, AtkUldComponentDataWindow> {

    /// <summary>
    /// Gets the area of the node minus the header size.
    /// </summary>
    public abstract Vector2 ContentSize { get; }

    /// <summary>
    /// Gets the position where the content starts, below the header node.
    /// </summary>
    public abstract Vector2 ContentStartPosition { get; }

    /// <summary>
    /// Gets the height of the header.
    /// </summary>
    public abstract float HeaderHeight { get; }

    /// <summary>
    /// Gets the node that should be focused.
    /// </summary>
    public abstract ResNode WindowHeaderFocusNode { get; }

    /// <summary>
    /// Sets the nodes title and subtitle.
    /// </summary>
    public virtual void SetTitle(string title, string? subtitle = null)
        => Component->SetTitle(title, subtitle ?? string.Empty);

    /// <summary>
    /// Constructs a new <see cref="WindowNodeBase"/>
    /// </summary>
    protected WindowNodeBase()
        => SetInternalComponentType(ComponentType.Window);
}
