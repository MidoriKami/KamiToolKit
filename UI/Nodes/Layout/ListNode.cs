using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Layout;
using KamiToolKit.Internal.Classes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Parts;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;
using ComponentNodeBase = KamiToolKit.Core.Primitives.ComponentNode.ComponentNode;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A virtual list that only allocates one node per <see cref="IListItemNode.ItemHeight"/>,
/// as the list is scrolled the same nodes are recycled to show the new values.
/// This is far more efficient than creating new nodes in runtime, and more efficient when showing large lists.
/// Please avoid using <see cref="VerticalListNode"/> with more than 30 items, and use this node instead.
/// </summary>
/// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
public unsafe class ListNode<T, TU> : ResNode, IControllerNavigable, IListNode<T, TU>, ILayoutFiller where TU : ListItemNode<T>, IListItemNode, new() {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ListNavNode? UpwardsNavNode { get; private set; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ListNavNode? DownwardsNavNode { get; private set; }

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
    public int NavIndex { get; set; }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

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
    public void ResetScroll() {
        scrollPosition = 0;
        ScrollBarNode.ScrollPosition = 0;
        PopulateNodes();
        RefreshNavigationForDynamicContent();
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
        UpwardsNavNode?.Dispose();
        UpwardsNavNode = null;

        DownwardsNavNode?.Dispose();
        DownwardsNavNode = null;

        foreach (var node in nodeList) {
            node.Dispose();
        }
        nodeList.Clear();

        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(Options.Count - nodeCount, 0));
        SelectedOptions.Clear();

        UpwardsNavNode = new ListNavNode {
            Position = new Vector2(0.0f, 0.0f),
            Size = new Vector2(Width, 4.0f),
            OnUpNavReceived = OnUpNavReceived,
            IsVisible = false,
        };
        UpwardsNavNode.AttachNode(this);

        RebuildNodeList();

        DownwardsNavNode = new ListNavNode {
            Position = new Vector2(0.0f, Height - 4.0f),
            Size = new Vector2(Width, 4.0f),
            OnDownNavReceived = OnDownNavReceived,
            IsVisible = false,
        };
        DownwardsNavNode.AttachNode(this);

        PopulateNodes();
        RecalculateScroll();
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

        UpwardsNavNode?.Width = Width;
        DownwardsNavNode?.Width = Width;

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

    /// <inheritdoc/>
    public override void UpdateNavigation() => UpdateNavigationFromRoot();

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        ArgumentNullException.ThrowIfNull(context);

        if (UpwardsNavNode is not null) {
            UpwardsNavNode.IsVisible = CanScrollUp;
        }

        if (DownwardsNavNode is not null) {
            DownwardsNavNode.IsVisible = CanScrollDown;
        }

        base.UpdateNavigation(context);
        ConnectConsecutiveRowBoundaries(context);
        ConnectScrollWindowBoundaries(context);
        UpdateNavigationBoundary();
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
        var maxScrollPosition = nodeList.Count is 0 ? 0 : Math.Max(0, Options.Count - nodeList.Count);
        var previousScrollPosition = scrollPosition;
        scrollPosition = Math.Clamp(scrollPosition, 0, maxScrollPosition);
        if (scrollPosition != previousScrollPosition) {
            PopulateNodes();
        }

        if (Options.Count < nodeCount) {
            ScrollBarNode.ScrollPosition = 0;
            ScrollBarNode.IsEnabled = false;
        }

        var totalHeight = (int)(Options.Count * (itemHeight + ItemSpacing) + ItemSpacing);
        ScrollBarNode.UpdateScrollParams((int)(nodeList.Count * (itemHeight + ItemSpacing)), totalHeight);
        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));

        RefreshNavigationForDynamicContent();
    }

    private void UpdateNavigationBoundary() {
        var rowBoundaries = nodeList
            .Where(node => node.IsVisible)
            .Select(node => node.NavigationBoundary)
            .Where(boundary => boundary.Top.Count is not 0 && boundary.Bottom.Count is not 0)
            .ToList();

        var entryTargets = rowBoundaries
            .Select(boundary => boundary.Top)
            .FirstOrDefault(targets => targets.Count is not 0) ?? [];

        if (entryTargets.Count is 0) {
            NavigationBoundary = ControllerNavigationBoundary.Empty;
            return;
        }

        var bottomBoundary = CanScrollDown ? [] : rowBoundaries[^1].Bottom.ToList();
        if (!CanScrollDown && nodeList.LastOrDefault(node => node.IsVisible && node.NavigationBoundary.Bottom.Count is not 0)?.NavigationCheckboxNode is { } bottomCheckbox) {
            bottomBoundary.Add(bottomCheckbox);
        }

        var leftBoundary = rowBoundaries.SelectMany(boundary => boundary.Left).Distinct().ToList();
        var rightBoundary = rowBoundaries.SelectMany(boundary => boundary.Right).Distinct().ToList();
        var topExit = new List<ComponentNodeBase>();
        if (!CanScrollUp) {
            topExit.AddRange(entryTargets);
            if (nodeList.FirstOrDefault(node => node.IsVisible && node.NavigationBoundary.Top.Count is not 0)?.NavigationCheckboxNode is { } topCheckbox) {
                topExit.Add(topCheckbox);
            }
        }

        NavigationBoundary = new ControllerNavigationBoundary(entryTargets, bottomBoundary, leftBoundary, rightBoundary, topExit, entryTargets);
    }

    private void ConnectScrollWindowBoundaries(ControllerNavigationContext context) {
        var rowBoundaries = nodeList
            .Where(node => node.IsVisible)
            .Select(node => node.NavigationBoundary)
            .Where(boundary => boundary.Top.Count is not 0 && boundary.Bottom.Count is not 0)
            .ToList();
        if (rowBoundaries.Count is 0) return;

        if (CanScrollUp && UpwardsNavNode is not null) {
            foreach (var component in rowBoundaries[0].Top) {
                context.TrySetLink(component, ControllerNavigationDirection.Up, UpwardsNavNode);
            }

            if (nodeList.FirstOrDefault(node => node.IsVisible && node.NavigationBoundary.Top.Count is not 0)?.NavigationCheckboxNode is { } topCheckbox) {
                context.TrySetLink(topCheckbox, ControllerNavigationDirection.Up, UpwardsNavNode);
            }
        }

        if (CanScrollDown && DownwardsNavNode is not null) {
            foreach (var component in rowBoundaries[^1].Bottom) {
                context.TrySetLink(component, ControllerNavigationDirection.Down, DownwardsNavNode);
            }

            if (nodeList.LastOrDefault(node => node.IsVisible && node.NavigationBoundary.Bottom.Count is not 0)?.NavigationCheckboxNode is { } bottomCheckbox) {
                context.TrySetLink(bottomCheckbox, ControllerNavigationDirection.Down, DownwardsNavNode);
            }
        }
    }

    private void ConnectConsecutiveRowBoundaries(ControllerNavigationContext context) {
        var rows = nodeList
            .Where(node => node.IsVisible)
            .Where(node => node.NavigationBoundary.Top.Any(component => component.NavIndex is >= 1 and <= byte.MaxValue)
                           && node.NavigationBoundary.Bottom.Any(component => component.NavIndex is >= 1 and <= byte.MaxValue))
            .ToList();

        for (var index = 0; index < rows.Count - 1; index++) {
            var currentBoundary = rows[index].NavigationBoundary;
            var nextBoundary = rows[index + 1].NavigationBoundary;
            ConnectRowBoundaryTargets(context, currentBoundary.Bottom, nextBoundary.Top, ControllerNavigationDirection.Down);
            ConnectRowBoundaryTargets(context, nextBoundary.Top, currentBoundary.Bottom, ControllerNavigationDirection.Up);

            if (rows[index].NavigationCheckboxNode is { } currentCheckbox) {
                ConnectRowBoundaryTargets(context, [currentCheckbox], nextBoundary.Top, ControllerNavigationDirection.Down);
            }

            if (rows[index + 1].NavigationCheckboxNode is { } nextCheckbox) {
                ConnectRowBoundaryTargets(context, [nextCheckbox], currentBoundary.Bottom, ControllerNavigationDirection.Up);
            }
        }
    }

    private static void ConnectRowBoundaryTargets(
        ControllerNavigationContext context,
        IReadOnlyList<ComponentNodeBase> sources,
        IReadOnlyList<ComponentNodeBase> targets,
        ControllerNavigationDirection direction) {

        var sourceGroups = sources
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .GroupBy(component => component.NavIndex)
            .Select(group => group.ToList())
            .ToList();
        var targetGroups = targets
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .GroupBy(component => component.NavIndex)
            .Select(group => group.ToList())
            .ToList();

        foreach (var sourceGroup in sourceGroups) {
            var targetDistances = targetGroups
                .Select(targetGroup => (
                    Components: targetGroup,
                    Distance: sourceGroup
                        .SelectMany(source => targetGroup.Select(target => MathF.Abs(GetCenterX(source) - GetCenterX(target))))
                        .Min()))
                .ToList();

            if (targetDistances.Count is 0) continue;

            var closestDistance = targetDistances.Min(target => target.Distance);
            var closestTargets = targetDistances
                .Where(target => MathF.Abs(target.Distance - closestDistance) <= 0.5f)
                .ToList();

            if (closestTargets.Count is not 1) {
                foreach (var source in sourceGroup) {
                    context.TrySetLink(source, direction, null);
                }
                context.ReportAmbiguousTarget($"{direction} list-row boundary from NavIndex {sourceGroup[0].NavIndex}");
                continue;
            }

            foreach (var source in sourceGroup) {
                context.TrySetLink(source, direction, closestTargets[0].Components[0]);
            }
        }
    }

    private static float GetCenterX(ComponentNodeBase component)
        => component.ScreenPosition.X + component.Width * component.Scale.X / 2.0f;

    private void RefreshNavigationForDynamicContent() {
        UpdateNavigationFromRoot();
    }

    private void OnScrollUpdate(int newPosition) {
        var newScrollPosition = (int)(newPosition / (itemHeight + ItemSpacing));
        var maxScrollPosition = nodeList.Count is 0 ? 0 : Math.Max(0, Options.Count - nodeList.Count);
        scrollPosition = Math.Clamp(newScrollPosition, 0, maxScrollPosition);
        PopulateNodes();
        RefreshNavigationForDynamicContent();

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
        }
    }

    private void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(0, Options.Count - nodeCount));

        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
        PopulateNodes();
        RefreshNavigationForDynamicContent();

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
        }

        atkEvent->SetEventIsHandled();
    }

    private void OnUpNavReceived() {
        if (!CanScrollUp) return;

        var currentBoundary = nodeList[0].NavigationBoundary.Top;
        scrollPosition--;
        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
        PopulateNodes();
        RefreshNavigationForDynamicContent();
        FocusAdjacentRow(currentBoundary, nodeList[0].NavigationBoundary.Top);
    }

    private void OnDownNavReceived() {
        if (!CanScrollDown) return;

        var currentBoundary = nodeList[^1].NavigationBoundary.Bottom;
        scrollPosition++;
        ScrollBarNode.ScrollPosition = (int)(scrollPosition * (itemHeight + ItemSpacing));
        PopulateNodes();
        RefreshNavigationForDynamicContent();
        FocusAdjacentRow(currentBoundary, nodeList[^1].NavigationBoundary.Bottom);
    }

    private static void FocusAdjacentRow(IReadOnlyList<ComponentNodeBase> currentBoundary, IReadOnlyList<ComponentNodeBase> nextBoundary) {
        if (currentBoundary.Count is 0 || nextBoundary.Count is 0) return;

        var currentCenter = GetCenterX(currentBoundary[0]);
        var target = nextBoundary
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .OrderBy(component => MathF.Abs(GetCenterX(component) - currentCenter))
            .FirstOrDefault();
        if (target is IFocusable focusable) {
            focusable.SetFocus();
        }
    }

    private bool CanScrollUp => nodeList.Count is not 0 && scrollPosition > 0;

    private bool CanScrollDown => nodeList.Count is not 0 && scrollPosition < Math.Max(0, Options.Count - nodeList.Count);

    private readonly List<TU> nodeList = [];
    private readonly float itemHeight;
    private int scrollPosition;
    private int nodeCount;
}
