using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A virtual list that only allocates one node per <see cref="IListItemNode.ItemHeight"/>,
/// as the list is scrolled the same nodes are recycled to show the new values.
/// This is far more efficient than creating new nodes in runtime, and more efficient when showing large lists.
/// Please avoid using <see cref="VerticalListNode"/> with more than 30 items, and use this node instead.
/// </summary>
/// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
public unsafe class ListNode<T, TU> : ResNode, IControllerNavigable where TU : ListItemNode<T>, IListItemNode, new() {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarNode ScrollBarNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode NoResultsTextNode { get; }

    /// <inheritdoc/>
    public int NavIndex { get; set; }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

    /// <summary>
    /// Gets or sets the string to show when there are no items in the list.
    /// </summary>
    public ReadOnlySeString? NoResultsString { get; set; }

    /// <summary>
    /// Gets or sets the action to be invoked when an item is selected.
    /// </summary>
    public Action<T?>? OnItemSelected { get; set; }

    /// <summary>
    /// Gets or sets the item spacing.
    /// </summary>
    public float ItemSpacing {
        get;
        set {
            field = value;
            FullRebuild();
        }
    }

    /// <summary>
    /// Gets or sets the options list that is used for rendering.
    /// </summary>
    public required List<T> OptionsList {
        get;
        set {
            field = value;

            var newNodeCount = (int)(Height / (itemHeight + ItemSpacing));
            if (newNodeCount != nodeCount) {
                FullRebuild();
            }
            else {
                PopulateNodes();
                RecalculateScroll();
            }

            if (NoResultsString is { } warningString && value.Count is 0) {
                NoResultsTextNode.String = warningString;
                NoResultsTextNode.IsVisible = true;
            }
            else {
                NoResultsTextNode.IsVisible = false;
            }
        }
    } = [];

    /// <summary>
    /// Resets scroll position back to the top.
    /// </summary>
    /// <remarks>
    /// When changing the list data, you will likely need to invoke this manually.
    /// </remarks>
    public void ResetScroll() {
        scrollPosition = 0;
        ScrollBarNode.ScrollPosition = 0;
        PopulateNodes();
    }

    /// <summary>
    /// Clears the currently selected item.
    /// </summary>
    public void ClearSelection() {
        if (selectedItem is not null) {
            selectedItem = default;
        }
    }

    /// <summary>
    /// Updates the data being displayed. This is done efficiently.
    /// </summary>
    public void Update() {
        PopulateNodes();

        foreach (var node in nodeList) {
            if (node.IsVisible) {
                node.Update();
            }
        }
    }

    /// <summary>
    /// Resets and rebuilds list
    /// </summary>
    /// <remarks>
    /// Use sparingly, this is an expensive operation.
    /// </remarks>
    public void FullRebuild() {
        foreach (var node in nodeList) {
            node.Dispose();
        }
        nodeList.Clear();

        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(OptionsList.Count - nodeCount, 0));
        selectedItem = default;

        RebuildNodeList();
        PopulateNodes();
        RecalculateScroll();
    }

    public ListNode() {
        itemHeight = TU.ItemHeight;

        ScrollBarNode = new ScrollBarNode {
            OnValueChanged = OnScrollUpdate,
            ScrollSpeed = (int)itemHeight,
            HideWhenDisabled = true,
        };
        ScrollBarNode.AttachNode(this);

        NoResultsTextNode = new TextNode {
            AlignmentType = AlignmentType.Center,
        };
        NoResultsTextNode.AttachNode(this);

        AddEvent(AtkEventType.MouseWheel, OnMouseWheel);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ScrollBarNode.Size = new Vector2(8.0f, Height);
        ScrollBarNode.Position = new Vector2(Width - 8.0f, 0.0f);

        NoResultsTextNode.Size = new Vector2(Width - 8.0f, Height);
        NoResultsTextNode.Position = Vector2.Zero;

        var newNodeCount = (int)(Height / (itemHeight + ItemSpacing));
        if (newNodeCount != nodeCount) {
            FullRebuild();
        }

        foreach (var node in nodeList) {
            node.Width = ScrollBarNode.Bounds.Left - 8.0f;
        }

        RecalculateScroll();
    }

    private void RebuildNodeList() {
        nodeCount = (int)(Height / (itemHeight + ItemSpacing));
        if (nodeCount < 1) return;

        foreach (var index in Enumerable.Range(0, nodeCount)) {
            var node = new TU {
                Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, itemHeight),
                Position = new Vector2(0.0f, index * (itemHeight + ItemSpacing)),
                NodeId = (uint)index + 2,
                OnClick = clickedNode => {
                    SelectItem(((TU)clickedNode).ItemData);
                    OnItemSelected?.Invoke(selectedItem);
                },
                IsVisible = false,
            };

            node.AttachNode(this);
            nodeList.Add(node);
        }
    }

    private void PopulateNodes() {
        foreach (var (nodeIndex, node) in nodeList.Index()) {
            var dataIndex = scrollPosition + nodeIndex;

            if (dataIndex < OptionsList.Count) {
                var item = OptionsList[dataIndex];
                node.ItemData = item;
                node.IsVisible = true;
                node.IsSelected = GenericUtil.AreEqual(item, selectedItem);
            }
            else {
                node.IsVisible = false;
            }
        }
    }

    private void SelectItem(T? item) {
        if (item is null) return;

        selectedItem = item;

        foreach (var node in nodeList) {
            if (node.ItemData is null) {
                node.IsSelected = false;
            }
            else {
                node.IsSelected = GenericUtil.AreEqual(node.ItemData, selectedItem);
            }
        }
    }

    private void RecalculateScroll() {
        if (OptionsList.Count < nodeCount) {
            ScrollBarNode.ScrollPosition = 0;
            ScrollBarNode.IsEnabled = false;
        }

        // Recalculate Controller Nav
        foreach (var index in Enumerable.Range(0, nodeList.Count)) {
            if (NavIndex is not 0) {
                var node = nodeList[index];

                // We'll allocate 4 nav slots per list item to do sub-nav via left/right.
                if (index is 0) {
                    node.ProcessNav(index * 4 + NavIndex, NavUp, (index + 1) * 4 + NavIndex);
                }
                else if (index == nodeCount - 1) {
                    node.ProcessNav(index * 4 + NavIndex, (index - 1) * 4 + NavIndex, nodeList.Count * 4 + NavIndex);
                }
                else {
                    node.ProcessNav(index * 4 + NavIndex, (index - 1) * 4 + NavIndex, (index + 1) * 4 + NavIndex);
                }
            }
        }

        var totalHeight = (int)(OptionsList.Count * (itemHeight + ItemSpacing) + ItemSpacing);
        ScrollBarNode.UpdateScrollParams((int)(nodeList.Count * (itemHeight + ItemSpacing)), totalHeight);
        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
    }

    private void OnScrollUpdate(int newPosition) {
        scrollPosition = (int)(newPosition / (itemHeight + ItemSpacing));
        PopulateNodes();
    }

    private void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(0, OptionsList.Count - nodeCount));

        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
        PopulateNodes();

        atkEvent->SetEventIsHandled();
    }

    private readonly List<TU> nodeList = [];
    private readonly float itemHeight;
    private T? selectedItem;
    private int scrollPosition;
    private int nodeCount;
}
