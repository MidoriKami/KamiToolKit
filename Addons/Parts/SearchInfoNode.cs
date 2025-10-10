using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addons.Parts;

internal unsafe class SearchInfoNode<T> : SimpleComponentNode {
    private readonly NineGridNode hoveredNineGridNode;
    private readonly NineGridNode selectedNineGridNode;
    
    private readonly IconImageNode iconNode;
    private readonly TextNode labelTextNode;
    private readonly TextNode subLabelTextNode;
    private readonly TextNode idTextNode;

    private CustomEventListener? eventListener;

    public SearchInfoNode() {
        eventListener = new CustomEventListener(HandleEvents);
        
        hoveredNineGridNode = new SimpleNineGridNode {
            NodeId = 2,
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        hoveredNineGridNode.AttachNode(this);

        selectedNineGridNode = new SimpleNineGridNode {
            NodeId = 3,
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        selectedNineGridNode.AttachNode(this);

        iconNode = new IconImageNode {
            IsVisible = true,
            FitTexture = true,
            IconId = 60072,
        };
        iconNode.AttachNode(this);

        labelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            FontSize = 14,
            AlignmentType = AlignmentType.BottomLeft,
            IsVisible = true,
        };
        labelTextNode.AttachNode(this);

        subLabelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            FontSize = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = KnownColor.Gray.Vector(),
        };
        subLabelTextNode.AttachNode(this);

        idTextNode = new TextNode {
            FontSize = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor = KnownColor.Gray.Vector(),
            IsVisible = true,
        };
        idTextNode.AttachNode(this);
        
        CollisionNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseOver,
            0,
            null,
            (AtkEventTarget*)CollisionNode.InternalResNode,
            eventListener.EventListener,
            false);

        CollisionNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseOut,
            0,
            null,
            (AtkEventTarget*)CollisionNode.InternalResNode,
            eventListener.EventListener,
            false);
        
        CollisionNode.InternalResNode->AtkEventManager.RegisterEvent(
            AtkEventType.MouseClick,
            0,
            null,
            (AtkEventTarget*)CollisionNode.InternalResNode,
            eventListener.EventListener,
            false);

        CollisionNode.DrawFlags |= DrawFlags.ClickableCursor;
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        eventListener?.Dispose();
        eventListener = null;
    }

    private void HandleEvents(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        switch (eventType) {
            case AtkEventType.MouseOver:
                IsHovered = true;
                break;
            
            case AtkEventType.MouseOut:
                IsHovered = false;
                break;
            
            case AtkEventType.MouseClick:
                OnClicked(this);
                break;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        hoveredNineGridNode.Size = Size;
        selectedNineGridNode.Size = Size;

        iconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        iconNode.Position = new Vector2(2.0f, 2.0f);

        labelTextNode.Size = new Vector2(Width - Height - 2.0f - 30.0f, Height / 2.0f);
        labelTextNode.Position = new Vector2(Height + 2.0f, 0.0f);

        subLabelTextNode.Size = new Vector2(Width - Height - 2.0f - 10.0f, Height / 2.0f);
        subLabelTextNode.Position = new Vector2(Height + 2.0f + 10.0f, Height / 2.0f);

        idTextNode.Size = new Vector2(30.0f, Height / 2.0f);
        idTextNode.Position = new Vector2(Width - 30.0f, 0.0f);
    }

    public required OptionInfo<T> OptionInfo {
        get;
        init {
            field = value;

            labelTextNode.String = value.Label;
            subLabelTextNode.String = value.SubLabel ?? string.Empty;
            idTextNode.String = value.Id?.ToString() ?? string.Empty;

            if (value.IconId is { } iconId) {
                iconNode.IconId = iconId;
            }

            if (!value.TexturePath.IsNullOrEmpty()) {
                iconNode.LoadTexture(value.TexturePath);
            }
        }
    }

    public bool IsHovered {
        get => hoveredNineGridNode.IsVisible;
        set => hoveredNineGridNode.IsVisible = value;
    }
    
    public bool IsSelected {
        get => selectedNineGridNode.IsVisible;
        set => selectedNineGridNode.IsVisible = value;
    }
    
    public required Action<SearchInfoNode<T>> OnClicked { get; init; }
}
