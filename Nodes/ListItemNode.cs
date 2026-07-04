using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;

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
                    SetNodeData(value);
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
    /// Function that is called when this list item entry needs to update what data is being displayed.
    /// </summary>
    /// <param name="itemData">The new item data to show.</param>
    protected abstract void SetNodeData(T itemData);

    /// <summary>
    /// Processes building controller navigation.
    /// </summary>
    public virtual void ProcessNav(int index, int up, int down, int left, int right) { }

    /// <summary>
    /// Prevents this selectable node from being highlightable or clickable.
    /// <see cref="SelectableNode.OnClick"/> will no longer be invoked.
    /// </summary>
    protected void DisableInteractions() {
        InteractionsDisabled = true;

        EnableSelection = false;
        EnableHighlight = false;

        RemoveEvent(AtkEventType.MouseOver);
        RemoveEvent(AtkEventType.MouseOut);
        RemoveEvent(AtkEventType.MouseDown);

        RemoveNodeFlags(NodeFlags.HasCollision);
        RemoveDrawFlags(DrawFlags.ClickableCursor);
    }

    /// <summary>
    /// Gets a value indicating whether internations with this node should be allowed.
    /// </summary>
    public bool InteractionsDisabled { get; private set; }
}
