using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Layout;
using KamiToolKit.Internal.Classes;
using KamiToolKit.UI.Nodes.Components;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A virtual list that only allocates one node per <see cref="IListItemNode.ItemHeight"/>,
/// as the list is scrolled the same nodes are recycled to show the new values.
/// This is far more efficient than creating new nodes in runtime, and more efficient when showing large lists.
/// Please avoid using <see cref="VerticalListNode"/> with more than 30 items, and use this node instead.
/// </summary>
/// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
public unsafe class ListNode<T, TU> : ResNode, IListNode<T, TU>, ILayoutFiller, INavigationUpdatable where TU : ListItemNode<T>, IListItemNode, new() {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarNode ScrollBarNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ResNode NoResultsTextNodeContainer { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode NoResultsTextNode { get; }

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Horizontal;

    /// <inheritdoc/>
    public bool ShowPlaceholderString { get; set; } = true;

    /// <inheritdoc/>
    public ReadOnlySeString PlaceholderString {
        get => NoResultsTextNode.String;
        set => NoResultsTextNode.String = value;
    }

    /// <inheritdoc/>
    public uint PlaceholderStringTextId {
        get => NoResultsTextNode.TextId;
        set => NoResultsTextNode.TextId = value;
    }

    /// <inheritdoc/>
    public NodeData.SheetType PlaceHolderStringSheetType {
        get => NoResultsTextNode.SheetType;
        set => NoResultsTextNode.SheetType = value;
    }

    /// <inheritdoc/>
    public Action<T>? OnOptionSelected { get; set; }

    /// <inheritdoc/>
    public Action<TU, T>? OnItemStateChanged { get; set; }

    /// <inheritdoc/>
    public object? NodeReferenceData { get; set; }

    /// <inheritdoc/>
    public List<T> SelectedOptions { get; private set; } = [];

    /// <inheritdoc/>
    public float ItemSpacing {
        get;
        set {
            field = value;
            FullRebuild();
        }
    }

    /// <inheritdoc/>
    public bool AutoResetScroll { get; set; } = true;

    /// <inheritdoc/>
    public bool EnableSelection { get; set; } = true;

    /// <inheritdoc/>
    public List<T> Options {
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

            NoResultsTextNodeContainer.IsVisible = value.Count is 0 && ShowPlaceholderString;

            if (AutoResetScroll) {
                ResetScroll();
            }
        }
    } = [];


    /// <inheritdoc/>
    public IReadOnlyList<TU> OptionNodes => nodeList;

    /// <inheritdoc/>
    public bool AllowMultipleSelection { get; set; }

    /// <inheritdoc/>
    public void UpdateNavigation(NavigationContext context) {
        // todo: this and stuff
    }

    /// <inheritdoc/>
    public void ResetScroll() {
        scrollPosition = 0;
        ScrollBarNode.ScrollPosition = 0;
        PopulateNodes();
    }

    /// <inheritdoc/>
    public void ClearSelection() {
        SelectedOptions.Clear();
        PopulateNodes();
    }

    /// <inheritdoc/>
    public void Update() {
        NoResultsTextNodeContainer.IsVisible = !NoResultsTextNode.String.IsEmpty && Options.Count is 0 && ShowPlaceholderString;

        PopulateNodes();

        foreach (var node in nodeList) {
            if (node.IsVisible) {
                node.Update();
            }
        }
    }

    /// <inheritdoc/>
    public void FullRebuild() {
        foreach (var node in nodeList) {
            node.Dispose();
        }
        nodeList.Clear();

        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(Options.Count - nodeCount, 0));
        SelectedOptions.Clear();

        RebuildNodeList();

        PopulateNodes();
        RecalculateScroll();
        // LayoutNode.RefreshNavigation(this); // todo: this?
    }

    /// <summary>
    /// Constructs a new <see cref="ListNode{T, TU}"/>
    /// </summary>
    public ListNode() {
        itemHeight = TU.ItemHeight;

        ScrollBarNode = new ScrollBarNode {
            OnScrollChanged = OnScrollUpdate,
            ScrollSpeed = (int)itemHeight,
            HideWhenDisabled = true,
        };
        ScrollBarNode.AttachNode(this);

        NoResultsTextNodeContainer = new ResNode {
            IsVisible = false,
        };
        NoResultsTextNodeContainer.AttachNode(this);

        NoResultsTextNode = new TextNode {
            AlignmentType = AlignmentType.Center,
            TextId = 5494, // "No results found."
            SheetType = NodeData.SheetType.Addon,
        };
        NoResultsTextNode.AttachNode(NoResultsTextNodeContainer);

        AddEvent(AtkEventType.MouseWheel, OnMouseWheel);
        AddNodeFlags(NodeFlags.HasCollision);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ScrollBarNode.Size = new Vector2(8.0f, Height);
        ScrollBarNode.Position = new Vector2(Width - 8.0f, 0.0f);

        NoResultsTextNodeContainer.Size = new Vector2(Width - 8.0f, Height);
        NoResultsTextNodeContainer.Position = Vector2.Zero;

        NoResultsTextNode.Size = NoResultsTextNodeContainer.Size;
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
                EnableSelection = EnableSelection,
                Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, itemHeight),
                Position = new Vector2(0.0f, index * (itemHeight + ItemSpacing)),
                OnClick = clickedNode => {
                    var itemData = ((TU)clickedNode).ItemData;

                    if (itemData is not null) {
                        SelectItem(itemData);
                        OnOptionSelected?.Invoke(itemData);
                    }
                },
                IsVisible = false,
                ReferenceData = NodeReferenceData,
            };

            node.OnStateChanged = _ => {
                if (node.ItemData is not null) {
                    OnItemStateChanged?.Invoke(node, node.ItemData);
                }
            };

            node.AttachNode(this);
            nodeList.Add(node);
        }
    }

    private void PopulateNodes() {
        foreach (var (nodeIndex, node) in nodeList.Index()) {
            var dataIndex = scrollPosition + nodeIndex;

            if (dataIndex < Options.Count) {
                var item = Options[dataIndex];
                node.ReferenceData = NodeReferenceData;
                node.ItemData = item;
                node.IsVisible = true;
                node.ShowClickableCursor = !node.InteractionsDisabled;
                node.IsSelected = SelectedOptions.Any(selectedItem => GenericUtil.AreEqual(node.ItemData, selectedItem));
                node.Refresh();
            }
            else {
                node.IsVisible = false;
                node.ShowClickableCursor = false;
            }
        }
    }

    private void SelectItem(T? item) {
        if (item is null) return;

        if (!AllowMultipleSelection) {
            SelectedOptions.Clear();
        }

        if (!SelectedOptions.Remove(item)) {
            SelectedOptions.Add(item);
        }

        foreach (var node in nodeList) {
            if (node.ItemData is null && !AllowMultipleSelection) {
                node.IsSelected = false;
            }
            else {
                node.IsSelected = SelectedOptions.Any(selectedItem => GenericUtil.AreEqual(node.ItemData, selectedItem));
            }
        }
    }

    private void RecalculateScroll() {
        if (Options.Count < nodeCount) {
            ScrollBarNode.ScrollPosition = 0;
            ScrollBarNode.IsEnabled = false;
        }

        var totalHeight = (int)(Options.Count * (itemHeight + ItemSpacing) + ItemSpacing);
        ScrollBarNode.UpdateScrollParams((int)(nodeList.Count * (itemHeight + ItemSpacing)), totalHeight);
        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
    }

    private void OnScrollUpdate(int newPosition) {
        scrollPosition = (int)(newPosition / (itemHeight + ItemSpacing));
        PopulateNodes();
        // LayoutNode.RefreshNavigation(this); // todo: this?

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
        }
    }

    private void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(0, Options.Count - nodeCount));

        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
        PopulateNodes();
        // LayoutNode.RefreshNavigation(this); // todo: this?

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
        }

        atkEvent->SetEventIsHandled();
    }


    private readonly List<TU> nodeList = [];
    private readonly float itemHeight;
    private int scrollPosition;
    private int nodeCount;

}
