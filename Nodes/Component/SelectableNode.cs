using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Nodes;

public class SelectableNode : SimpleComponentNode {
    private readonly NineGridNode hoveredBackgroundNode;
    private readonly NineGridNode selectedBackgroundNode;

    public SelectableNode() {
        hoveredBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        hoveredBackgroundNode.AttachNode(this);
        
        selectedBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        selectedBackgroundNode.AttachNode(this);
        
        CollisionNode.AddEvent(AtkEventType.MouseOver, () => {
            if (!IsSelected) {
                IsHovered = true;
            }
        });
        CollisionNode.AddEvent(AtkEventType.MouseDown, () => {
            if (EnableSelection) {
                IsSelected = true;
                OnClick?.Invoke(this);
            }
        });
        CollisionNode.AddEvent(AtkEventType.MouseOut, () => {
            IsHovered = false;
        });
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        hoveredBackgroundNode.Size = Size + new Vector2(6.0f, 6.0f);
        hoveredBackgroundNode.Position = new Vector2(-3.0f, -3.0f);
        
        selectedBackgroundNode.Size = Size + new Vector2(6.0f, 6.0f);
        selectedBackgroundNode.Position = new Vector2(-3.0f, -3.0f);
    }

    public Action<SelectableNode>? OnClick {
        get;
        set {
            field = value;
            CollisionNode.ShowClickableCursor = value is not null && EnableSelection;
        }
    }

    public bool EnableSelection {
        get;
        set {
            field = value;
            CollisionNode.ShowClickableCursor = value;
        }
    } = true;

    public bool IsHovered {
        get => hoveredBackgroundNode.IsVisible;
        set => hoveredBackgroundNode.IsVisible = value;
    }
    
    public bool IsSelected {
        get => selectedBackgroundNode.IsVisible;
        set {
            selectedBackgroundNode.IsVisible = value;

            if (value) {
                hoveredBackgroundNode.IsVisible = false;
            }
        }
    }
}
