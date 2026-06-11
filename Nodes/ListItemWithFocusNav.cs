using System.Numerics;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract list item node for use with implementations of <see cref="ListItemNode{T}"/>
/// that don't use any component nodes to use with navigation.
/// </summary>
public abstract class ListItemWithFocusNav<T> : ListItemNode<T> {

    /// <summary>
    /// The focus node the cursor will use for interaction.
    /// </summary>
    protected NavFocusNode NavFocusNode { get; }

    /// <summary>
    /// Overridable function that is called when this node is selected.
    /// </summary>
    protected virtual void OnNavSelected() {
        IsSelected = !IsSelected;
        OnClick?.Invoke(this);
    }

    /// <summary>
    /// Overridable function that is called when this node is hovered.
    /// </summary>
    protected virtual void OnNavHoverStart() { }

    /// <summary>
    /// Overridable function that is called when this node is no longer hovered.
    /// </summary>
    protected virtual void OnNavHoverEnd() { }

    protected ListItemWithFocusNav() {
        NavFocusNode = new NavFocusNode {
            OnSelected = OnNavSelected,
            OnHoverStart = OnNavHoverStart,
            OnHoverEnd = OnNavHoverEnd,
        };
        NavFocusNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        NavFocusNode.Size = new Vector2(0.0f, 0.0f);
        NavFocusNode.Position = new Vector2(2.0f, Height / 2.0f);
    }

    public override void ProcessNav(int index, int up, int down, int left, int right) {
        base.ProcessNav(index, up, down, left, right);

        NavFocusNode.NavIndex = index;
        NavFocusNode.NavUp = up;
        NavFocusNode.NavDown = down;
        NavFocusNode.NavLeft = left;
        NavFocusNode.NavRight = right;
    }
}
