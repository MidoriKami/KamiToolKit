using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Primitives.NodeBase;
using KamiToolKit.Internal.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.UI.Nodes.Layout;

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
    public Vector4 TextColor {
        get => ToggleableHeaderNode.TextColor;
        set => ToggleableHeaderNode.TextColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => ToggleableHeaderNode.TextOutlineColor;
        set => ToggleableHeaderNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => ToggleableHeaderNode.String;
        set => ToggleableHeaderNode.String = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => ToggleableHeaderNode.AlignmentType;
        set => ToggleableHeaderNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => ToggleableHeaderNode.FontType;
        set => ToggleableHeaderNode.FontType = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => ToggleableHeaderNode.TextFlags;
        set => ToggleableHeaderNode.TextFlags = value;
    }

    /// <inheritdoc />
    public uint FontSize {
        get => ToggleableHeaderNode.FontSize;
        set => ToggleableHeaderNode.FontSize = value;
    }

    /// <inheritdoc />
    public bool StretchWidth { get; set; }

    /// <summary>
    /// Constructs a new <see cref="CollapsingHeaderNode"/>
    /// </summary>
    public CollapsingHeaderNode() {
        ToggleableHeaderNode = new ToggleableHeaderNode();
        ToggleableHeaderNode.AttachNode(this);

        ToggleableHeaderNode.OnCollapsed += UpdateLayout;
        ToggleableHeaderNode.OnUncollapsed += UpdateLayout;
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ToggleableHeaderNode.Size = new Vector2(Width, 28.0f);
        ToggleableHeaderNode.Position = new Vector2(0.0f, 0.0f);
    }

    /// <inheritdoc />
    protected override void OnUpdateSizes() {
        if (IsCollapsed) {
            Height = 28.0f;
        }
        else {
            foreach (var node in NodeList) {
                if (StretchWidth) {
                    node.Width = Width;
                }
            }
            Height = CalculateCollapsedHeight();
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        foreach (var node in NodeList) {
            node.IsVisible = !IsCollapsed;
        }

        if (!IsCollapsed) {
            var yPosition = ToggleableHeaderNode.Height + FirstItemSpacing;

            foreach (var node in NodeList) {
                node.Y = yPosition;
                yPosition += node.Height + ItemSpacing;
            }
        }
    }

    /// <summary>
    /// Calculates the height of the node when not collapsed, based on the header and child nodes.
    /// </summary>
    private float CalculateCollapsedHeight() {
        var yPosition = ToggleableHeaderNode.Height + FirstItemSpacing;

        foreach (var node in NodeList) {
            yPosition += node.Height + ItemSpacing;
        }

        return yPosition;
    }

    /// <inheritdoc />
    protected override void OnUpdateControllerNav() {
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
