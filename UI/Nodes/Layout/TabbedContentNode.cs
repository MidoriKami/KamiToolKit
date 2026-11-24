using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Primitives.NodeBase;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// This node represents a tab bar and multiple VerticalListNode's representing the tab contents.
/// </summary>
public sealed class TabbedContentNode : LayoutListNode, ITabbedContentNode {

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
        // Not implemented yet.
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
