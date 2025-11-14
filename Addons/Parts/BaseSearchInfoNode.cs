using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addons.Parts;

public abstract unsafe class BaseSearchInfoNode<T> : SimpleComponentNode {
    protected readonly NineGridNode HoveredNineGridNode;
    protected readonly NineGridNode SelectedNineGridNode;

    protected readonly IconImageNode IconNode;
    protected readonly TextNode LabelTextNode;
    protected readonly TextNode SubLabelTextNode;
    protected readonly TextNode IdTextNode;

    private CustomEventListener? eventListener;

    public required T Option {
        get;
        set {
            field = value;
            SetOptionParams(value);
        }
    }

    protected BaseSearchInfoNode() {
        eventListener = new CustomEventListener(HandleEvents);
        
        HoveredNineGridNode = new SimpleNineGridNode {
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
        HoveredNineGridNode.AttachNode(this);

        SelectedNineGridNode = new SimpleNineGridNode {
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
        SelectedNineGridNode.AttachNode(this);

        IconNode = new IconImageNode {
            FitTexture = true,
            IconId = 60072,
        };
        IconNode.AttachNode(this);

        LabelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 14,
            LineSpacing = 14,
            AlignmentType = AlignmentType.BottomLeft,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        LabelTextNode.AttachNode(this);

        SubLabelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis | TextFlags.Emboss,
            FontSize = 12,
            LineSpacing = 12,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        SubLabelTextNode.AttachNode(this);

        IdTextNode = new TextNode {
            TextFlags = TextFlags.Emboss,
            FontSize = 10,
            AlignmentType = AlignmentType.BottomRight,
            TextColor = KnownColor.Gray.Vector(),
        };
        IdTextNode.AttachNode(this);

        CollisionNode.DrawFlags |= DrawFlags.ClickableCursor;
        CollisionNode.AddEvent(AtkEventType.MouseOver, HandleEvents);
        CollisionNode.AddEvent(AtkEventType.MouseOut, HandleEvents);
        CollisionNode.AddEvent(AtkEventType.MouseClick, HandleEvents);
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            eventListener?.Dispose();
            eventListener = null;
        }
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
                OnClicked?.Invoke(this);
                break;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        HoveredNineGridNode.Size = Size;
        SelectedNineGridNode.Size = Size;

        IconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        IconNode.Position = new Vector2(2.0f, 2.0f);

        LabelTextNode.Size = new Vector2(Width - Height - 2.0f - 30.0f, Height / 2.0f);
        LabelTextNode.Position = new Vector2(Height + 2.0f, 0.0f);

        SubLabelTextNode.Size = new Vector2(Width - Height - 2.0f - 10.0f, Height / 2.0f);
        SubLabelTextNode.Position = new Vector2(Height + 2.0f + 10.0f, Height / 2.0f);

        IdTextNode.Size = new Vector2(30.0f, Height / 2.0f);
        IdTextNode.Position = new Vector2(Width - 30.0f, 0.0f);
    }

    protected abstract void SetOptionParams(T? value);
    public Action<BaseSearchInfoNode<T>>? OnClicked { get; set; }
    
    public bool IsHovered {
        get => HoveredNineGridNode.IsVisible;
        set => HoveredNineGridNode.IsVisible = value;
    }

    public bool IsSelected {
        get => SelectedNineGridNode.IsVisible;
        set => SelectedNineGridNode.IsVisible = value;
    }

    public abstract int Compare(BaseSearchInfoNode<T> other, string sortOption, bool reversed);

    public abstract bool IsMatch(string searchString);

    public void Refresh() => SetOptionParams(Option);
}
