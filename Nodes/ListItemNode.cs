using KamiToolKit.Classes.Internal;

namespace KamiToolKit.Nodes;

/// <summary>
/// Abstract class for use with <see cref="ListNode{T,TU}"/>.
/// </summary>
public abstract class ListItemNode<T> : SelectableNode {

    /// <summary>
    /// Gets or sets the item data for this node.
    /// </summary>
    public T? ItemData {
        get;
        set {
            if (value is not null) {
                if (!GenericUtil.AreEqual(field, value)) {
                    IsSettingNodeData = true;
                    SetNodeData(value);
                    IsSettingNodeData = false;
                }
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
    /// Bool that indicates if SetNodeData when different is being called.
    /// Used to prevent things like checkboxes from trigger a file save due to the value being changed.
    /// </summary>
    protected bool IsSettingNodeData { get; private set; }

    /// <summary>
    /// Function that is called when this list item entry needs to update what data is being displayed.
    /// </summary>
    /// <param name="itemData">The new item data to show.</param>
    protected abstract void SetNodeData(T itemData);

    protected void DisableInteractions() {
        EnableSelection = false;
        EnableHighlight = false;
    }
}
