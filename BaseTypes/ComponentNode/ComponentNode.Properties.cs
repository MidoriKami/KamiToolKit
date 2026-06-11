using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace KamiToolKit.BaseTypes.ComponentNode;

/// <summary>
/// Abstract implementation of the base class for all component nodes use in KamiToolKit.
/// </summary>
public abstract unsafe partial class ComponentNode(NodeType nodeType) : NodeBase<AtkComponentNode>(nodeType) {

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

    /// <summary>
    /// Gets or sets the index of this nodes controller nav.
    /// </summary>
    public int NavIndex {
        get => CursorNavInfo.Index;
        set => CursorNavInfo.Index = (byte) value;
    }

    /// <summary>
    /// Gets or sets the index of the left direction nav.
    /// </summary>
    public int NavLeft {
        get => CursorNavInfo.LeftIndex;
        set => CursorNavInfo.LeftIndex = (byte) value;
    }

    /// <summary>
    /// Gets or sets the index of the right direction nav.
    /// </summary>
    public int NavRight {
        get => CursorNavInfo.RightIndex;
        set => CursorNavInfo.RightIndex = (byte) value;
    }

    /// <summary>
    /// Gets or sets the index of the up direction nav.
    /// </summary>
    public int NavUp {
        get => CursorNavInfo.UpIndex;
        set => CursorNavInfo.UpIndex = (byte) value;
    }

    /// <summary>
    /// Gets or sets the index of the down direction nav.
    /// </summary>
    public int NavDown {
        get => CursorNavInfo.DownIndex;
        set => CursorNavInfo.DownIndex = (byte) value;
    }

    /// <summary>
    /// Gets the node used when focusing this element via controller.
    /// </summary>
    public NodeBase FocusNode {
        get => field ?? CollisionNode;
        protected set;
    }
}
