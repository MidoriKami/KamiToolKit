using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// This node represents a tab bar and multiple VerticalListNode's representing the tab contents.
/// </summary>
public sealed class TabbedContentNode : LayoutListNode, ITabbedContentNode {

    private readonly List<ControllerNavigationContext> pageNavigationContexts = [];
    private int pageContextStartIndex;
    private bool pageContextStartIndexSet;

    /// <inheritdoc />
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Vertical;

    private readonly TabBarNode tabBarNode;

    /// <inheritdoc />
    public Action? OnTabChanged { get; set; }

    private IEnumerable<NodeBase> ContentNodes => NodeList.Skip(1);

    /// <summary>
    /// Constructs a new <see cref="TabbedContentNode"/>.
    /// </summary>
    public TabbedContentNode() {
        tabBarNode = new TabBarNode();
        AddNode(tabBarNode);

        Height = 24.0f;
    }

    /// <inheritdoc />
    protected override void OnUpdateSizes() {
        tabBarNode.Size = new Vector2(Width, 24.0f);

        var contentHeight = Math.Max(0.0f, Height - tabBarNode.Bounds.Bottom);

        foreach (var node in ContentNodes) {
            node.Size = new Vector2(Width, contentHeight);
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        tabBarNode.Position = new Vector2(0.0f, 0.0f);

        var contentY = tabBarNode.Bounds.Bottom;

        foreach (var node in ContentNodes) {
            node.Position = new Vector2(0.0f, contentY);
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
        var contentNodes = ContentNodes.ToList();
        var buttons = tabBarNode.Buttons;
        var pageBoundaries = contentNodes.Select(GetNavigationBoundaryForNode).ToList();
        var pageEntryTargets = pageBoundaries.Select(GetNavigationEntryTargets).ToList();

        for (var index = 0; index < Math.Min(contentNodes.Count, buttons.Count); index++) {
            ConnectNavigationBoundaryTargets([buttons[index]], pageEntryTargets[index],
                ControllerNavigationDirection.Down, ControllerNavigationDirection.Up, vertical: true, connectReverse: false);

            foreach (var target in pageBoundaries[index].TopExit) {
                CurrentNavigationContext.TrySetLink(target, ControllerNavigationDirection.Up, buttons[index]);
            }
        }

        var nonEmptyPageEntries = pageEntryTargets.Where(targets => targets.Count is not 0).ToList();
        if (nonEmptyPageEntries.Count is not 0) {
            var sharedEntryIndices = nonEmptyPageEntries[0]
                .Select(target => target.NavIndex)
                .Where(navIndex => navIndex is >= 1 and <= byte.MaxValue)
                .ToHashSet();

            foreach (var pageTargets in nonEmptyPageEntries.Skip(1)) {
                sharedEntryIndices.IntersectWith(pageTargets
                    .Select(target => target.NavIndex)
                    .Where(navIndex => navIndex is >= 1 and <= byte.MaxValue));
            }

            var sharedEntryTargets = nonEmptyPageEntries[0]
                .Where(target => sharedEntryIndices.Contains(target.NavIndex))
                .GroupBy(target => target.NavIndex)
                .Select(group => group.First())
                .ToList();

            foreach (var button in buttons) {
                var buttonCenterX = button.ScreenPosition.X + button.Width * button.Scale.X / 2.0f;
                var target = sharedEntryTargets
                    .OrderBy(entry => MathF.Abs(buttonCenterX - (entry.ScreenPosition.X + entry.Width * entry.Scale.X / 2.0f)))
                    .FirstOrDefault();

                if (target is not null) {
                    CurrentNavigationContext.TrySetLink(button, ControllerNavigationDirection.Down, target);
                }
            }
        }

        var boundary = CreateNavigationBoundary(GetNavigationChildGroups());
        if (buttons.Count is not 0) {
            var rightBoundary = pageBoundaries
                .SelectMany(pageBoundary => pageBoundary.Right)
                .Distinct()
                .ToList();
            var rightEntryTargets = pageBoundaries
                .SelectMany(pageBoundary => pageBoundary.RightEntryTargets)
                .Distinct()
                .ToList();

            boundary = new ControllerNavigationBoundary([buttons[0]], boundary.Bottom, [buttons[0]], rightBoundary, buttons, rightEntryTargets);
        }

        SetNavigationBoundary(boundary);
    }

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        pageNavigationContexts.Clear();
        pageContextStartIndexSet = false;

        base.UpdateNavigation(context);

        if (pageNavigationContexts.Count > 0) {
            context.TryReserveThrough(pageNavigationContexts.Max(pageContext => pageContext.NextIndex));
        }
    }

    /// <inheritdoc/>
    protected override ControllerNavigationContext CreateChildNavigationContext(NodeBase child, ControllerNavigationContext context) {
        if (ReferenceEquals(child, tabBarNode)) return context;

        if (!pageContextStartIndexSet) {
            pageContextStartIndex = context.NextIndex;
            pageContextStartIndexSet = true;
        }

        var pageContext = context.CreateLocalScope(pageContextStartIndex);
        pageNavigationContexts.Add(pageContext);
        return pageContext;
    }

    /// <summary>
    /// Adds an existing node as a tab page and connects its entry boundary to the new tab button.
    /// </summary>
    public void AddTab(TabBarEntry entry, NodeBase content) {
        var index = ContentNodes.Count();
        content.IsVisible = index is 0;
        AddNode(content);

        tabBarNode.AddTab(new TabBarEntry {
            Label = entry.Label,
            Tooltip = entry.Tooltip,
            TextId = entry.TextId,
            SheetType = entry.SheetType,
            OnClick = () => {
                HandleTabClick(index);
                entry.OnClick?.Invoke();
            },
        });
    }

    /// <inheritdoc />
    public void AddTab(ReadOnlySeString label, out VerticalListNode tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null) {
        var index = NodeList.Count - 1;

        tabContents = new VerticalListNode {
            StretchWidth = true,
            IsVisible = index is 0,
        };
        AddNode(tabContents);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    /// <inheritdoc />
    public void AddTab<T>(ReadOnlySeString label, out T tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase, new() {
        var index = NodeList.Count - 1;

        tabContents = new T();
        tabContents.IsVisible = index is 0;
        AddNode(tabContents);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    /// <inheritdoc />
    public void AddTab<T>(ReadOnlySeString label, T contentNode, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase {
        var index = NodeList.Count - 1;

        contentNode.IsVisible = index is 0;

        AddNode(contentNode);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    private void HandleTabClick(int selectedIndex) {
        foreach (var (index, node) in ContentNodes.Index()) {
            node.IsVisible = index == selectedIndex;
        }

        OnTabChanged?.Invoke();
    }
}
