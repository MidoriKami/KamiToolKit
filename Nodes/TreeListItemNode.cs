namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract class for use with <see cref="TreeListNode{T,TU}"/>
/// </summary>
public abstract class TreeListItemNode<T> : SelectableNode {
    /// <summary>
    /// Gets or sets the item data for this node.
    /// </summary>
    public T? ItemData {
        get;
        set {
            if (value is not null) {
                SetNodeData(value);
            }

            field = value;

            IsVisible = value is not null;
        }
    }

    /// <summary>
    /// Update function that is called each frame the list is called to be updated.
    /// </summary>
    /// <remarks>
    /// This can be useful for displaying list data that changes as the player moves for example.
    /// </remarks>
    public virtual void Update() { }

    /// <summary>
    /// Function that is called when this list item entry needs to update what data is being displayed.
    /// </summary>
    /// <param name="itemData">The new item data to show.</param>
    protected abstract void SetNodeData(T itemData);

    /// <summary>
    /// Processes building controller navigation.
    /// </summary>
    public virtual void ProcessNav(int index, int up, int down, int left, int right) { }
}
