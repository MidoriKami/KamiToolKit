using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Inheritable node that manages hovered and clicked states.
/// </summary>
public abstract class SelectableNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    private NineGridNode HoveredBackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    private NineGridNode SelectedBackgroundNode { get; }

    /// <summary>
    /// Gets or sets the actions that is invoked when this node is clicked. Provides a reference to itself.
    /// </summary>
    public Action<SelectableNode>? OnClick {
        get;
        set {
            field = value;
            ShowClickableCursor = value is not null && EnableSelection;
        }
    }

    /// <summary>
    /// Gets or sets whether this node is allowed to be selected.
    /// </summary>
    public bool EnableSelection {
        get;
        set {
            field = value;
            ShowClickableCursor = value;
        }
    } = true;

    /// <summary>
    /// Gets or sets whether this node will highlight upon hover.
    /// </summary>
    public bool EnableHighlight { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this node is hovered.
    /// </summary>
    public bool IsHovered {
        get => HoveredBackgroundNode.IsVisible;
        set => HoveredBackgroundNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets whether this node is currently selected.
    /// </summary>
    public bool IsSelected {
        get => SelectedBackgroundNode.IsVisible;
        set {
            SelectedBackgroundNode.IsVisible = value;

            if (value) {
                HoveredBackgroundNode.IsVisible = false;
            }
        }
    }

    protected SelectableNode() {
        HoveredBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        HoveredBackgroundNode.AttachNode(this);

        SelectedBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        SelectedBackgroundNode.AttachNode(this);

        AddEvent(AtkEventType.MouseOver, () => {
            if (!IsSelected && EnableHighlight) {
                IsHovered = true;
            }
        });

        AddEvent(AtkEventType.MouseDown, () => {
            if (EnableSelection) {
                IsSelected = !IsSelected;
                OnClick?.Invoke(this);
            }
        });

        AddEvent(AtkEventType.MouseOut, () => {
            IsHovered = false;
        });

        AddDrawFlags(DrawFlags.ClickableCursor);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        HoveredBackgroundNode.Size = Size + new Vector2(6.0f, 6.0f);
        HoveredBackgroundNode.Position = new Vector2(-3.0f, -3.0f);

        SelectedBackgroundNode.Size = Size + new Vector2(6.0f, 6.0f);
        SelectedBackgroundNode.Position = new Vector2(-3.0f, -3.0f);
    }
}
