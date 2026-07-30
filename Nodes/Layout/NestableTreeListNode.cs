using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Internal.Nodes;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Virtualized scrollable tree list with nested collapsible sections via <see cref="Sections"/>.
/// </summary>
/// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
/// <remarks>Overall more inline with ListNode than TreeListNode.</remarks>
public class NestableTreeListNode<T, TU> : ResNode where TU : TreeListItemNode<T>, ITreeListItemNode, new() {

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
    /// When updating <see cref="Sections"/>, automatically resets scroll to the top.
    /// </summary>
    /// <remarks>
    /// This may be undesirable if the list is being constantly updated.
    /// </remarks>
    public bool AutoResetScroll { get; set; } = true;

    /// <summary>
    /// Gets or sets the nested section tree used to populate this list.
    /// </summary>
    public List<TreeListSection<T>> Sections {
        get;
        set {
            field = value;
            OnDataChanged(value.Count is 0);
        }
    } = [];

    /// <summary>
    /// Gets a read-only list of the pooled entry nodes.
    /// </summary>
    public IReadOnlyList<TU> EntryNodes => entryNodes;

    /// <summary>
    /// Gets or sets the item spacing.
    /// </summary>
    public float ItemSpacing {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the string to show when there are no items in the list.
    /// </summary>
    public ReadOnlySeString? NoResultsString {
        get;
        set {
            field = value;
            if (value is { } stringValue) {
                NoResultsTextNode.String = stringValue;
            }
            else {
                NoResultsTextNode.String = string.Empty;
            }
        }
    }

    /// <summary>
    /// Resets scroll position back to the top.
    /// </summary>
    /// <remarks>
    /// When changing the list data, you will probably need to invoke this manually if <see cref="AutoResetScroll"/> is disabled.
    /// </remarks>
    public void ResetScroll() {
        scrollPosition = 0;
        ScrollBarNode.ScrollPosition = 0;
        PopulateNodes();
    }

    /// <summary>
    /// Updates the data being displayed.
    /// </summary>
    public void Update() {
        NoResultsTextNodeContainer.IsVisible = !NoResultsTextNode.String.IsEmpty && Sections.Count is 0;

        PopulateNodes();

        foreach (var node in entryNodes) {
            if (node.IsVisible) {
                node.Update();
            }
        }
    }

    /// <summary>
    /// Constructs a new instance of <see cref="NestableTreeListNode{T,TU}"/>
    /// </summary>
    public unsafe NestableTreeListNode() {
        itemHeight = TU.ItemHeight;

        ScrollBarNode = new ScrollBarNode {
            OnValueChanged = OnScrollUpdate,
            ScrollSpeed = (int)itemHeight,
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

        RebuildNodes();
        PopulateNodes();
    }

    private void OnDataChanged(bool isEmpty) {
        NoResultsTextNodeContainer.IsVisible = isEmpty;

        RebuildNodes();

        if (AutoResetScroll) {
            ResetScroll();
        }
        else {
            PopulateNodes();
        }
    }

    private IEnumerable<NestableTreeListRow<T>> EnumerateVisibleRows() {
        foreach (var section in Sections) {
            foreach (var row in EnumerateSectionRows(section, parentPath: default)) {
                yield return row;
            }
        }
    }

    private IEnumerable<NestableTreeListRow<T>> EnumerateSectionRows(TreeListSection<T> section, ReadOnlySeString parentPath) {
        var path = parentPath.IsEmpty ? section.Header : $"{parentPath}/{section.Header}";
        yield return NestableTreeListRow<T>.ForGroup(section.Header, path);

        if (CollapsedEntries.Contains(path)) {
            yield break;
        }

        foreach (var entry in section.Entries) {
            yield return NestableTreeListRow<T>.ForEntry(entry);
        }

        foreach (var child in section.Children) {
            foreach (var row in EnumerateSectionRows(child, path)) {
                yield return row;
            }
        }
    }

    /// <summary>
    /// Function is called on any click-drag of the scrollbar, or direct mousewheel on the scrollbar.
    /// </summary>
    private unsafe void OnScrollUpdate(int newPosition) {
        var remainingPosition = (float) newPosition;
        var scrollOffset = 0;

        foreach (var row in EnumerateVisibleRows()) {
            remainingPosition -= RowHeight(row) + ItemSpacing;

            if (remainingPosition <= 0) {
                scrollPosition = scrollOffset;
                PopulateNodes();
                return;
            }

            scrollOffset++;
        }

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
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

        var numValidOptions = EnumerateVisibleRows().Count();

        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, numValidOptions - Math.Min(HeaderNodes.Count, entryNodes.Count));
        ScrollBarNode.ScrollPosition = (float) scrollPosition / numValidOptions * GetTotalOffscreenHeight();

        PopulateNodes();

        if (ParentAddon is not null) {
            ParentAddon->UpdateCollisionNodeList(false);
        }

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
            HeaderCollapsePaths.Clear();

            foreach (var _ in Enumerable.Range(0, headerNodeCount)) {
                var headerNode = new ToggleableHeaderNode {
                    Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, 28.0f),
                    Position = new Vector2(0.0f, -32.0f),
                    IsVisible = false,
                };

                HeaderCollapsePaths.Add(default);
                var capturedIndex = HeaderNodes.Count;

                headerNode.OnToggle = isVisible => {
                    var path = HeaderCollapsePaths[capturedIndex];
                    if (path.IsEmpty) return;

                    if (isVisible) {
                        CollapsedEntries.Remove(path);
                    }
                    else if (!CollapsedEntries.Contains(path)) {
                        CollapsedEntries.Add(path);
                    }

                    PopulateNodes();
                };

                headerNode.AttachNode(this);
                HeaderNodes.Add(headerNode);
            }
        }

        var entryNodeCount = (int) (Height / (itemHeight + ItemSpacing));
        if (entryNodeCount != entryNodes.Count) {
            foreach (var node in entryNodes) {
                node.Dispose();
            }
            entryNodes.Clear();

            foreach (var _ in Enumerable.Range(0, entryNodeCount)) {
                var node = new TU {
                    Size = new Vector2(ScrollBarNode.Bounds.Left - 8.0f, itemHeight),
                    OnClick = clickedNode => {
                        entryNodes.ForEach(entry => entry.IsSelected = false);

                        clickedNode.IsSelected = true;

                        SelectedItem = ((TU)clickedNode).ItemData;
                        OnItemSelected?.Invoke(SelectedItem);
                    },
                    IsVisible = false,
                };

                node.AttachNode(this);
                entryNodes.Add(node);
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
            node.Y = 0.0f;
            node.IsVisible = false;
            node.Height = 0.0f;
        });

        entryNodes.ForEach(node => {
            node.Y = 0.0f;
            node.IsVisible = false;
            node.Height = 0.0f;
        });

        var position = 0.0f;

        // To handle scroll position, we have to skip a certain number of entries and
        // sub entries according to the collapsed state of each header.
        var scrollSkips = scrollPosition;

        foreach (var row in EnumerateVisibleRows()) {
            var rowHeight = RowHeight(row);
            if (position + rowHeight + ItemSpacing > Height) break;

            if (scrollSkips is 0 || scrollSkips-- <= 0) {
                switch (row.Kind) {
                    case NestableTreeListRowKind.Group:
                        if (headerIndex >= HeaderNodes.Count) {
                            goto done;
                        }

                        var headerNode = HeaderNodes[headerIndex];
                        HeaderCollapsePaths[headerIndex] = row.Path;
                        headerIndex++;

                        headerNode.Height = 28.0f;
                        headerNode.String = row.Header;
                        headerNode.IsVisible = true;
                        headerNode.IsCollapsed = CollapsedEntries.Contains(row.Path);
                        headerNode.Y = position;
                        break;

                    case NestableTreeListRowKind.Entry:
                        if (entryIndex >= entryNodes.Count) {
                            goto done;
                        }

                        var entryNode = entryNodes[entryIndex];
                        entryIndex++;

                        entryNode.Height = itemHeight;
                        entryNode.ItemData = row.Entry!;
                        entryNode.IsVisible = true;
                        entryNode.IsSelected = GenericUtil.AreEqual(entryNode.ItemData, SelectedItem);
                        entryNode.Y = position;
                        break;
                }

                position += rowHeight + ItemSpacing;
            }
        }

        done:
        ScrollBarNode.UpdateScrollParams((int) ScrollBarNode.Height, (int) GetTotalOffscreenHeight());
    }

    private float GetTotalOffscreenHeight() {
        var calculatedOffscreenHeight = itemHeight + ItemSpacing;

        foreach (var row in EnumerateVisibleRows()) {
            calculatedOffscreenHeight += RowHeight(row) + ItemSpacing;
        }

        return calculatedOffscreenHeight;
    }

    private float RowHeight(NestableTreeListRow<T> row)
        => row.Kind is NestableTreeListRowKind.Group ? 28.0f : itemHeight;

    private List<ToggleableHeaderNode> HeaderNodes { get; } = [];
    private List<ReadOnlySeString> HeaderCollapsePaths { get; } = [];
    private List<ReadOnlySeString> CollapsedEntries { get; } = [];

    private readonly List<TU> entryNodes = [];
    private readonly float itemHeight;

    private int scrollPosition;
}
