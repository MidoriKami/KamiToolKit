using System;
using System.Numerics;
using KamiToolKit.Interfaces;
using KamiToolKit.Internal.Nodes;

namespace KamiToolKit.Nodes;

/// <summary>
/// Layout node representing a collapsing header that will collapse/hide its contained nodes.
/// </summary>
public class CollapsingHeaderNode : LayoutListNode, ICollapsingHeaderNode {

    /// <inheritdoc />
    public bool IsCollapsed => ToggleableHeaderNode.IsCollapsed;

    /// <inheritdoc />
    public Action? OnCollapsed {
        get => ToggleableHeaderNode.OnCollapsed;
        set => ToggleableHeaderNode.OnCollapsed = value;
    }

    /// <inheritdoc />
    public Action? OnUncollapsed {
        get => ToggleableHeaderNode.OnUncollapsed;
        set => ToggleableHeaderNode.OnUncollapsed = value;
    }

    /// <inheritdoc />
    public Action<bool>? OnCollapseToggled {
        get => ToggleableHeaderNode.OnCollapseToggled;
        set => ToggleableHeaderNode.OnCollapseToggled = value;
    }

    /// <inheritdoc />
    public bool StretchWidth { get; set; }

    /// <summary>
    /// Constructs a new <see cref="CollapsingHeaderNode"/>
    /// </summary>
    public CollapsingHeaderNode() {
        ToggleableHeaderNode = new ToggleableHeaderNode();
        ToggleableHeaderNode.AttachNode(this);

        ToggleableHeaderNode.OnCollapsed += () => {
            foreach (var node in Nodes) {
                node.IsVisible = false;
            }
            Height = 28.0f;
        };

        ToggleableHeaderNode.OnUncollapsed += () => {
            foreach (var node in Nodes) {
                node.IsVisible = true;
            }
            RecalculateLayout();
        };
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ToggleableHeaderNode.Size = new Vector2(Width, 28.0f);
        ToggleableHeaderNode.Position = new Vector2(0.0f, 0.0f);
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        if (IsCollapsed) {
            foreach (var node in Nodes) {
                node.IsVisible = false;
            }

            Height = 28.0f;
        }
        else {
            var yPosition = ToggleableHeaderNode.Height + FirstItemSpacing;

            foreach (var node in Nodes) {
                node.IsVisible = !IsCollapsed;
                node.Y = yPosition;

                yPosition += node.Height + ItemSpacing;

                if (StretchWidth) {
                    node.Width = Width;
                }
            }

            Height = yPosition;
        }
    }

    /// <inheritdoc />
    protected override void OnRecalculateNavigation() {
        // Not implemented yet.
    }

    /// <inheritdoc />
    public void Collapse(bool playSoundEffect = true)
        => ToggleableHeaderNode.Collapse(playSoundEffect);

    /// <inheritdoc />
    public void Uncollapse(bool playSoundEffect = true)
        => ToggleableHeaderNode.Uncollapse(playSoundEffect);

    /// <inheritdoc />
    public void Toggle(bool playSoundEffect = true)
        => ToggleableHeaderNode.Toggle(playSoundEffect);

    private ToggleableHeaderNode ToggleableHeaderNode { get; }
}
