using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Core.Primitives;

/// <summary>
/// Abstract implementation of the base class for all component nodes use in KamiToolKit.
/// </summary>
public abstract unsafe partial class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType), INavigationTarget {

    /// <summary>
    /// Gets the collision node used for this component node.
    /// </summary>
    public abstract CollisionNode CollisionNode { get; }

    /// <summary>
    /// Gets the base typed component pointer.
    /// </summary>
    public abstract AtkComponentBase* ComponentBase { get; }

    /// <summary>
    /// Gets the base typed uld data pointer.
    /// </summary>
    public abstract AtkUldComponentDataBase* DataBase { get; }

    /// <summary>
    /// Gets the cursor navigation info from the component.
    /// </summary>
    public ref AtkCursorNavigationInfo CursorNavInfo
        => ref ComponentBase->CursorNavigationInfo;

    /// <inheritdoc/>
    public byte NavIndex {
        get => CursorNavInfo.Index;
        set => CursorNavInfo.Index = value;
    }

    /// <inheritdoc/>
    public byte NavLeft {
        get => CursorNavInfo.LeftIndex;
        set => CursorNavInfo.LeftIndex = value;
    }

    /// <inheritdoc/>
    public byte NavRight {
        get => CursorNavInfo.RightIndex;
        set => CursorNavInfo.RightIndex = value;
    }

    /// <inheritdoc/>
    public byte NavUp {
        get => CursorNavInfo.UpIndex;
        set => CursorNavInfo.UpIndex = value;
    }

    /// <inheritdoc/>
    public byte NavDown {
        get => CursorNavInfo.DownIndex;
        set => CursorNavInfo.DownIndex = value;
    }

    /// <summary>
    /// Gets the node used when focusing this element via controller.
    /// </summary>
    public NodeBase FocusNode {
        get => field ?? CollisionNode;
        protected set;
    }
}
