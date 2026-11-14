using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.BaseTypes;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// This node represents a tab bar and multiple VerticalListNode's representing the tab contents.
/// </summary>
public sealed class TabbedContentNode : VerticalListNode, ITabbedContentNode {
    private readonly TabBarNode tabBarNode;
    private readonly List<NodeBase> contentNodes = [];

    /// <inheritdoc />
    public Action? OnTabChanged { get; set; }

    /// <summary>
    /// Constructs a new <see cref="TabbedContentNode"/>.
    /// </summary>
    public TabbedContentNode() {
        tabBarNode = new TabBarNode();
        tabBarNode.AttachNode(this);

        Height = 24.0f;
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        tabBarNode.Position = new Vector2(0.0f, 0.0f);
        tabBarNode.Size = new Vector2(Width, 24.0f);

        var contentHeight = Math.Max(0.0f, Height - tabBarNode.Bounds.Bottom);
        var contentSize = new Vector2(Width, contentHeight);

        foreach (var contentNode in contentNodes) {
            contentNode.Position = new Vector2(0.0f, tabBarNode.Bounds.Bottom);
            contentNode.Size = contentSize;

            if (contentNode is ILayoutUpdatable updatable) {
                updatable.RecalculateLayout();
            }
        }
    }

    /// <inheritdoc />
    public void AddTab(ReadOnlySeString label, out VerticalListNode tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null) {
        var index = contentNodes.Count;

        contentNodes.Add(tabContents = new VerticalListNode {
            StretchWidth = true,
            IsVisible = index is 0,
        });
        tabContents.AttachNode(this);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    /// <inheritdoc />
    public void AddTab<T>(ReadOnlySeString label, out T tabContents, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase, new() {
        var index = contentNodes.Count;

        contentNodes.Add(tabContents = new T());
        tabContents.AttachNode(this);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    /// <inheritdoc />
    public void AddTab<T>(ReadOnlySeString label, T contentNode, ReadOnlySeString? tooltip = null, Action? tabChanged = null) where T : NodeBase {
        var index = contentNodes.Count;

        contentNode.IsVisible = index is 0;

        contentNodes.Add(contentNode);
        contentNode.AttachNode(this);

        tabBarNode.AddTab(label, () => {
            HandleTabClick(index);
            tabChanged?.Invoke();
        }, tooltip);
    }

    private void HandleTabClick(int selectedIndex) {
        foreach (var (index, node) in contentNodes.Index()) {
            node.IsVisible = index == selectedIndex;
        }

        OnTabChanged?.Invoke();
    }
}
