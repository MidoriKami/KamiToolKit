using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;
using KamiToolKit.Internal.Classes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;
using KamiToolKit.UI.Nodes.Parts;
using ComponentNodeBase = KamiToolKit.Core.Primitives.ComponentNode.ComponentNode;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// Abstract class for use with <see cref="ListNode{T,TU}"/>.
/// </summary>
public abstract class ListItemNode<T> : SelectableNode {

    /// <summary>
    /// The focus component representing this list item itself.
    /// </summary>
    protected NavFocusNode RowFocusNode { get; }

    internal CheckboxNode? NavigationCheckboxNode { get; private set; }

    /// <summary>
    /// Constructs a list item with a dedicated controller focus component.
    /// </summary>
    protected ListItemNode() {
        RowFocusNode = new NavFocusNode();
        RowFocusNode.AttachNode(this);
        RowFocusNode.OnSelected = OnNavSelected;
        RowFocusNode.OnHoverStart = OnNavHoverStart;
        RowFocusNode.OnHoverEnd = OnNavHoverEnd;
    }

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
    /// Gets the list's reference data. Often this will be a reference to a configuration data object.
    /// </summary>
    public object? ReferenceData { get; set; }

    /// <summary>
    /// Forces this node to re-render its current item data using the current reference data.
    /// Use when the reference data changed but the item data remained the same.
    /// </summary>
    public void Refresh() {
        if (ItemData is not null) {
            SetNodeData(ItemData);
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
    public virtual void ProcessNav(int index, int up, int down, int left, int right) {
        RowFocusNode.NavIndex = index;
        RowFocusNode.NavUp = up;
        RowFocusNode.NavDown = down;
        RowFocusNode.NavLeft = left;
        RowFocusNode.NavRight = right;
    }

    /// <summary>
    /// Overridable function that is called when this node is selected through controller navigation.
    /// </summary>
    protected virtual void OnNavSelected() {
        IsSelected = !IsSelected;
        OnClick?.Invoke(this);
    }

    /// <summary>
    /// Overridable function that is called when this node is hovered through controller navigation.
    /// </summary>
    protected virtual void OnNavHoverStart() { }

    /// <summary>
    /// Overridable function that is called when this node is no longer hovered through controller navigation.
    /// </summary>
    protected virtual void OnNavHoverEnd() { }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        RowFocusNode.Size = Vector2.Zero;
        RowFocusNode.Position = new Vector2(2.0f, Height / 2.0f);
    }

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        base.UpdateNavigation(context);

        var navigationComponents = GetNavigationComponents(this)
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .ToList();
        NavigationCheckboxNode = navigationComponents.OfType<CheckboxNode>().FirstOrDefault();

        if (RowFocusNode.NavIndex is < 1 or > byte.MaxValue) {
            NavigationBoundary = ControllerNavigationBoundary.Empty;
            return;
        }

        if (NavigationCheckboxNode is { } checkboxNode) {
            context.TrySetLink(RowFocusNode, ControllerNavigationDirection.Right, checkboxNode);
            context.TrySetLink(checkboxNode, ControllerNavigationDirection.Left, RowFocusNode);
        }

        var rightBoundary = NavigationCheckboxNode is { } rightCheckbox
            ? new ComponentNodeBase[] { rightCheckbox }
            : Array.Empty<ComponentNodeBase>();

        NavigationBoundary = new ControllerNavigationBoundary(
            [RowFocusNode],
            [RowFocusNode],
            [RowFocusNode],
            rightBoundary,
            [RowFocusNode]);
    }

    private static IEnumerable<ComponentNodeBase> GetNavigationComponents(NodeBase node) {
        foreach (var child in node.ChildNodes) {
            if (child is ComponentNodeBase component) {
                yield return component;
            }
            else {
                foreach (var descendantComponent in GetNavigationComponents(child)) {
                    yield return descendantComponent;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a callback that can optionally be invoked to inform the containing list that
    /// this node's state was changed, and to trigger <see cref="ListNode{T,TU}.OnItemStateChanged"/>.
    /// </summary>
    public Action<ListItemNode<T>>? OnStateChanged { get; set; }

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
    /// Gets a value indicating whether interactions with this node should be allowed.
    /// </summary>
    public bool InteractionsDisabled { get; private set; }
}
