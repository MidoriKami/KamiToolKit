using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Internal.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Virtualized node representing a scrollable tree list, categories can be collapsed or uncollapsed as needed.
/// </summary>
/// /// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
public class TreeListNode<T, TU> : ResNode where TU : TreeListItemNode<T>, ITreeListItemNode, new()  {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarNode ScrollBarNode { get; }

    /// <summary>
    /// Action that is invoked when an option is clicked on.
    /// </summary>
    /// <remarks>
    /// This only applies to the list item nodes, and will not trigger when a header is clicked on.
    /// </remarks>
    public Action<T?>? OnItemSelected { get; set; }

    /// <summary>
    /// Gets or sets the selected node.
    /// </summary>
    public T? SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of options used to populate this <see cref="TreeListNode{T,TU}"/>
    /// </summary>
    /// <remarks>
    /// Keys represent collapsing headers, where values are the entries shown per header.
    /// </remarks>
    public Dictionary<ReadOnlySeString, List<T>> Options {
        get;
        set {
            field = value;
            RebuildNodes();
            PopulateNodes();
        }
    } = [];

    /// <summary>
    /// Gets or sets the item spacing.
    /// </summary>
    public float ItemSpacing {
        get;
        set;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="TreeListNode{T,TU}"/>
    /// </summary>
    public unsafe TreeListNode() {
        itemHeight = TU.ItemHeight;

        ScrollBarNode = new ScrollBarNode {
            OnValueChanged = OnScrollUpdate,
            ScrollSpeed = (int)itemHeight,
        };
        ScrollBarNode.AttachNode(this);

        AddEvent(AtkEventType.MouseWheel, OnMouseWheel);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ScrollBarNode.Size = new Vector2(8.0f, Height);
        ScrollBarNode.Position = new Vector2(Width - 8.0f, 0.0f);

        RebuildNodes();
        PopulateNodes();
    }

    /// <summary>
    /// Function is called on any click-drag of the scrollbar, or direct mousewheel on the scrollbar.
    /// </summary>
    private void OnScrollUpdate(int newPosition) {
        var remainingPosition = (float) newPosition;
        var scrollOffset = 0;

        foreach (var (_, entryList) in Options) {
            remainingPosition -= 28.0f + ItemSpacing;

            if (remainingPosition <= 0) {
                scrollPosition = scrollOffset;
                PopulateNodes();
                return;
            }

            scrollOffset++;

            foreach (var _ in entryList) {
                remainingPosition -= itemHeight + ItemSpacing;

                if (remainingPosition <= 0) {
                    scrollPosition = scrollOffset;
                    PopulateNodes();
                    return;
                }

                scrollOffset++;
            }
        }
    }

    /// <summary>
    /// Function is called when the content body is scrolled via mousewheel.
    /// </summary>
    private unsafe void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (!ScrollBarNode.IsEnabled) {
            atkEvent->SetEventIsHandled();
            return;
        }

        var numValidOptions = 0;

        foreach (var (header, entryList) in Options) {
            numValidOptions++;

            if (!CollapsedEntries.Contains(header)) {
                foreach (var _ in entryList) {
                    numValidOptions++;
                }
            }
        }

        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, numValidOptions - Math.Max(HeaderNodes.Count, EntryNodes.Count) + 1);
        ScrollBarNode.ScrollPosition = (int) ( (float) scrollPosition / numValidOptions * GetTotalOffscreenHeight());

        PopulateNodes();

        atkEvent->SetEventIsHandled();
    }

    /// <summary>
    /// Rebuilds the node arrays if needed, checks if the correct number are allocated.
    /// </summary>
    private void RebuildNodes() {
        var headerNodeCount = (int) (Height / (28.0f + ItemSpacing));
        if (headerNodeCount != HeaderNodes.Count) {
            foreach (var node in HeaderNodes) {
                node.Dispose();
            }
            HeaderNodes.Clear();

            foreach (var _ in Enumerable.Range(0, headerNodeCount)) {
                var headerNode = new ToggleableHeaderNode {
                    Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, 28.0f),
                    Position = new Vector2(0.0f, -32.0f),
                    IsVisible = false,
                };

                headerNode.OnToggle = isVisible => {
                    if (isVisible) {
                        CollapsedEntries.Remove(headerNode.String);
                    }
                    else {
                        if (!CollapsedEntries.Contains(headerNode.String)) {
                            CollapsedEntries.Add(headerNode.String);
                        }
                    }

                    PopulateNodes();
                };

                headerNode.AttachNode(this);
                HeaderNodes.Add(headerNode);
            }
        }

        var entryNodeCount = (int) (Height / (itemHeight + ItemSpacing));
        if (entryNodeCount != EntryNodes.Count) {
            foreach (var node in EntryNodes) {
                node.Dispose();
            }
            EntryNodes.Clear();

            foreach (var _ in Enumerable.Range(0, entryNodeCount)) {
                var node = new TU {
                    Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, itemHeight),
                    OnClick = clickedNode => {
                        EntryNodes.ForEach(node => node.IsSelected = false);

                        clickedNode.IsSelected = true;

                        SelectedItem = ((TU)clickedNode).ItemData;
                        OnItemSelected?.Invoke(SelectedItem);
                    },
                    IsVisible = false,
                };

                node.AttachNode(this);
                EntryNodes.Add(node);
            }
        }
    }

    /// <summary>
    /// Fills the data for each node, but also repositions them to make it seem seamless.
    /// </summary>
    private void PopulateNodes() {
        var headerIndex = 0;
        var entryIndex = 0;

        HeaderNodes.ForEach(node => {
            node.IsVisible = false;
            node.Height = 0.0f;
        });

        EntryNodes.ForEach(node => {
            node.IsVisible = false;
            node.Height = 0.0f;
        });

        var position = 0.0f;

        // To handle scroll position, we have to skip a certain number of entries and
        // sub entries according to the collapsed state of each header.
        var scrollSkips = scrollPosition;

        foreach (var (header, entries) in Options) {
            if (headerIndex > HeaderNodes.Count) break;
            if (position + 28.0f + ItemSpacing > Height) break;

            var isCollapsed = CollapsedEntries.Contains(header);

            if (scrollSkips is 0 || scrollSkips-- <= 0) {
                var headerNode = HeaderNodes[headerIndex];
                headerIndex++;

                headerNode.Height = 28.0f;
                headerNode.String = header;
                headerNode.IsVisible = true;
                headerNode.IsCollapsed = isCollapsed;

                headerNode.Y = position;
                position += headerNode.Height + ItemSpacing;
            }

            if (isCollapsed) continue;

            foreach (var entry in entries) {
                if (entryIndex > EntryNodes.Count) break;
                if (position + itemHeight + ItemSpacing > Height) break;

                if (scrollSkips is 0 || scrollSkips-- <= 0) {
                    var entryNode = EntryNodes[entryIndex];
                    entryIndex++;

                    entryNode.Height = itemHeight;
                    entryNode.ItemData = entry;
                    entryNode.IsVisible = true;
                    entryNode.IsSelected = GenericUtil.AreEqual(entryNode.ItemData, SelectedItem);

                    entryNode.Y = position;
                    position += entryNode.Height + ItemSpacing;
                }
            }
        }

        ScrollBarNode.UpdateScrollParams((int) ScrollBarNode.Height, (int) GetTotalOffscreenHeight());
    }

    private float GetTotalOffscreenHeight() {
        var calculatedOffscreenHeight = itemHeight + ItemSpacing;

        foreach (var (header, entryList) in Options) {
            calculatedOffscreenHeight += 28.0f + ItemSpacing;

            if (!CollapsedEntries.Contains(header)) {
                foreach (var _ in entryList) {
                    calculatedOffscreenHeight += itemHeight + ItemSpacing;
                }
            }
        }

        return calculatedOffscreenHeight;
    }

    private List<ToggleableHeaderNode> HeaderNodes { get; } = [];
    private List<TU> EntryNodes { get; } = [];
    private List<ReadOnlySeString> CollapsedEntries { get; } = [];

    private readonly float itemHeight;

    private int scrollPosition;
}
