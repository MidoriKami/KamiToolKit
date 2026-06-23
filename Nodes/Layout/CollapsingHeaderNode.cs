using System;
using System.Numerics;
using KamiToolKit.Internal.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Layout node representing a collapsing header that will collapse/hide its contained nodes.
/// </summary>
public class CollapsingHeaderNode : LayoutListNode {

    /// <summary>
    /// Gets or sets the nodes collapsed state.
    /// </summary>
    public bool IsCollapsed {
        get => ToggleableHeaderNode.IsCollapsed;
        set => ToggleableHeaderNode.IsCollapsed = value;
    }

    /// <summary>
    /// Gets or sets whether contained nodes will be resized to fit this node's width.
    /// </summary>
    public bool FitWidth { get; set; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => ToggleableHeaderNode.String;
        set => ToggleableHeaderNode.String = value;
    }

    /// <summary>
    /// Action that is invoked when the header node is collapsed.
    /// </summary>
    public Action? OnCollapse { get; set; }

    /// <summary>
    /// Action that is invoked when the header node is uncollapsed.
    /// </summary>
    public Action? OnUncollapse { get; set; }

    /// <summary>
    /// Action that is invoked when the header is either collapsed or uncollapsed.
    /// </summary>
    /// <remarks>
    /// Boolean indicates if the node contents are visible (the node is in the uncollapsed state)
    /// </remarks>
    public Action<bool>? OnToggle { get; set; }

    /// <summary>
    /// Constructs a new <see cref="CollapsingHeaderNode"/>
    /// </summary>
    public CollapsingHeaderNode() {
        ToggleableHeaderNode = new ToggleableHeaderNode {
            OnCollapse = Collapse,
            OnUncollapse = Uncollapse,
        };
        ToggleableHeaderNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ToggleableHeaderNode.Size = new Vector2(Width, 28.0f);
        ToggleableHeaderNode.Position = new Vector2(0.0f, 0.0f);
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        var yPosition = ToggleableHeaderNode.Height + FirstItemSpacing;

        foreach (var node in Nodes) {
            node.IsVisible = !IsCollapsed;
            node.Y = yPosition;

            yPosition += node.Height + ItemSpacing;

            if (FitWidth) {
                node.Width = Width;
            }
        }

        Height = yPosition;
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        // Not implemented yet.
    }

    private void Collapse() {
        foreach (var node in Nodes) {
            node.IsVisible = false;
        }

        Height = 28.0f;

        OnCollapse?.Invoke();
        OnToggle?.Invoke(false);
    }

    private void Uncollapse() {
        foreach (var node in Nodes) {
            node.IsVisible = true;
        }

        RecalculateLayout();

        OnUncollapse?.Invoke();
        OnToggle?.Invoke(true);
    }

    private ToggleableHeaderNode ToggleableHeaderNode { get; }
}
