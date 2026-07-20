using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Internal.Extensions;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Action = System.Action;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="DropDownNode{T}"/> for strings.
/// </summary>
public class StringDropDownNode : DropDownNode<string> {
    /// <inheritdoc />
    public StringDropDownNode()
        => GetLabelFunction = entry => entry;
}

/// <summary>
/// Specialization of <see cref="DropDownNode{T}"/> for enums.
/// </summary>
public class EnumDropDownNode<T> : DropDownNode<T> where T : Enum {
    /// <inheritdoc />
    public EnumDropDownNode()
        => GetLabelFunction = entry => entry.Description;
}

/// <summary>
/// Custom implementation of a native DropDownNode.
/// </summary>
/// <typeparam name="T">Data type this dropdown node is representing.</typeparam>
public unsafe class DropDownNode<T> : SimpleComponentNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImageNode CollapseArrowNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public CollisionNode DropDownFocusCollisionNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode LabelNode { get; }

    /// <summary>
    /// Gets whether this node is collapsed.
    /// </summary>
    public bool IsCollapsed { get; private set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of buttons to generate.
    /// </summary>
    /// <remarks>
    /// Defaults to 5 entries, adjust this if your list is long.
    /// </remarks>
    public int MaxListOptions {
        get;
        set {
            field = value;
            RebuildPopupList();
        }
    } = 5;

    /// <summary>
    /// Gets or sets the action that is invoked when an option is selected.
    /// </summary>
    public Action<T>? OnOptionSelected { get; set; }

    /// <summary>
    /// Gets or sets the action that is invoked when the collapse state changes.
    /// </summary>
    public Action<bool>? OnCollapseToggled { get; set; }

    /// <summary>
    /// Gets or sets the action that is invoked when the button list is uncollapsed.
    /// </summary>
    public Action? OnUncollapsed { get; set; }

    /// <summary>
    /// Gets or sets the action that is invoked when the button list is collapsed.
    /// </summary>
    public Action? OnCollapsed { get; set; }

    /// <summary>
    /// Gets or sets the options represented by this node.
    /// </summary>
    public List<T> Options {
        get;
        set {
            field = value;

            if (Options.Count is 0) {
                LabelNode.String = PlaceholderString ?? "Empty Options, No Placeholder";
            }
            else {
                if (PlaceholderString is { } placeholder) {
                    LabelNode.String = placeholder;
                }
            }

            RebuildPopupList();
        }
    } = [];

    /// <summary>
    /// Gets or sets the placeholder string to be shown when there is no option selected.
    /// </summary>
    public ReadOnlySeString? PlaceholderString {
        get;
        set {
            field = value;

            if (Options.Count is 0) {
                LabelNode.String = PlaceholderString ?? "Empty Options, No Placeholder";
            }
            else {
                if (PlaceholderString is { } placeholder) {
                    LabelNode.String = placeholder;
                }
                else {
                    UpdateLabel(SelectedOption);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the option that will be shown as the selected option.
    /// </summary>
    public T? SelectedOption {
        get;
        set {
            field = value;
            UpdateLabel(value);
        }
    }

    /// <summary>
    /// Collapses the button list.
    /// </summary>
    public void Collapse(bool playSoundEffect = true) {
        if (!IsEnabled) return;
        if (IsCollapsed) return;

        IsCollapsed = true;
        Timeline?.PlayAnimation(4);

        PopupContainerNode.IsVisible = false;

        PopupContainerNode.RemoveDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null) {
            ClearFocusable(ParentAddon);
        }

        if (playSoundEffect && Component->SoundEffectId is not -1) {
            Component->PlaySoundEffect();
        }

        DropDownFocusCollisionNode.ReattachNode(this);
        PopupContainerNode.ReattachNode(this);

        DropDownFocusCollisionNode.IsVisible = false;
        DropDownFocusCollisionNode.Size = new Vector2(0.0f, 0.0f);

        // Need to reset position after reattaching, so screen position is recalculated correctly
        PopupContainerNode.Position = Size with { X = 0.0f } + new Vector2(4.0f, -4.0f);

        OnCollapsed?.Invoke();
    }

    /// <summary>
    /// Uncollapses the button list.
    /// </summary>
    public void Uncollapse(bool playSoundEffect = true) {
        if (!IsEnabled) return;
        if (!IsCollapsed) return;

        IsCollapsed = false;
        Timeline?.PlayAnimation(11);

        scrollPosition = 0;
        PopulatePopupList();
        PopupScrollbarNode.ScrollPosition = 0;

        PopupContainerNode.IsVisible = true;
        PopupContainerNode.AddDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null) {
            SetFocusable(ParentAddon);
        }

        if (playSoundEffect && Component->SoundEffectId is not -1) {
            Component->PlaySoundEffect();
        }

        if (ParentAddon is not null) {
            PopupContainerNode.Position = (ScreenPosition - ParentAddon->Position) / ParentAddon->Scale + Size with { X = 0.0f } + new Vector2(4.0f, -4.0f);
            MoveListOnScreen();

            DropDownFocusCollisionNode.IsVisible = true;
            DropDownFocusCollisionNode.Position = new Vector2(0.0f);
            DropDownFocusCollisionNode.Size = ParentAddon->RootSize;

            DropDownFocusCollisionNode.ReattachNode(ParentAddon->RootNode);
            PopupContainerNode.ReattachNode(ParentAddon->RootNode);
        }

        OnUncollapsed?.Invoke();
    }

    /// <summary>
    /// Toggles the visibility of the button list.
    /// </summary>
    public void Toggle(bool playSoundEffect = true) {
        if (!IsEnabled) return;

        if (IsCollapsed) {
            Uncollapse(playSoundEffect);
        }
        else {
            Collapse(playSoundEffect);
        }

        OnCollapseToggled?.Invoke(IsCollapsed);
    }

    /// <summary>
    /// Function that is called to resolve an entries displayed label.
    /// </summary>
    public Func<T, ReadOnlySeString>? GetLabelFunction { get; set; }

    /// <summary>
    /// Constructs a new <see cref="DropDownNode{T}"/>.
    /// </summary>
    public DropDownNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/DropDownA.tex",
            TextureSize = new Vector2(44.0f, 23.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            Size = new Vector2(250.0f, 24.0f),
            Height = 23.0f,
            LeftOffset = 16.0f,
            RightOffset = 16.0f,
        };
        BackgroundNode.AttachNode(this);

        CollapseArrowNode = new SimpleImageNode {
            TexturePath = "ui/uld/DropDownA.tex",
            TextureCoordinates = new Vector2(44.0f, 0.0f),
            TextureSize = new Vector2(12.0f, 12.0f),
            Position = new Vector2(6.0f, 17.0f),
            Size = new Vector2(12.0f, 12.0f),
            WrapMode = WrapMode.Stretch,
        };
        CollapseArrowNode.AttachNode(this);

        LabelNode = new TextNode {
            Position = new Vector2(20.0f, 0.0f),
            Size = new Vector2(218.0f, 21.0f),
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(50),
            TextOutlineColor = ColorHelper.GetColor(7),
            TextFlags = TextFlags.Ellipsis,
        };
        LabelNode.AttachNode(this);

        // Manually set the placeholder text string, as using textId will overwrite our placeholder on attach.
        LabelNode.String = IDataManager.Get().GetExcelSheet<Addon>().GetRow(1818).Text;

        PopupContainerNode = new SimpleComponentNode {
            Position = new Vector2(4.0f, 21.0f),
            Size = new Vector2(242.0f, 243.0f),
            IsVisible = false,
        };
        PopupContainerNode.AttachNode(this);
        PopupContainerNode.AddEvent(AtkEventType.MouseWheel, OnMouseWheel);

        PopupBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListB.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 32.0f),
            TopOffset = 10,
            BottomOffset = 12,
            LeftOffset = 10,
            RightOffset = 10,
        };
        PopupBackgroundNode.AttachNode(PopupContainerNode);

        PopupScrollbarNode = new ScrollBarNode {
            OnValueChanged = OnScrollUpdate,
            ScrollSpeed = 22,
            HideWhenDisabled = true,
        };
        PopupScrollbarNode.AttachNode(PopupContainerNode);

        PopupButtonListNode = new VerticalListNode {
            FitWidth = true,
        };
        PopupButtonListNode.AttachNode(PopupContainerNode);

        DropDownFocusCollisionNode = new CollisionNode();
        DropDownFocusCollisionNode.AttachNode(PopupContainerNode, NodePosition.AfterTarget);

        DropDownFocusCollisionNode.AddEvent(AtkEventType.MouseDown, () => Toggle());
        DropDownFocusCollisionNode.AddEvent(AtkEventType.MouseWheel, () => Toggle());

        BuildTimelines();

        Timeline?.PlayAnimation(4);

        CollisionNode.ShowClickableCursor = true;
        CollisionNode.AddEvent(AtkEventType.MouseOver, () => Timeline?.PlayAnimation(IsCollapsed ? 2 : 9));
        CollisionNode.AddEvent(AtkEventType.MouseOut, () => Timeline?.PlayAnimation(IsCollapsed ? 4 : 11));
        CollisionNode.AddEvent(AtkEventType.MouseClick, () => Toggle());

        Component->SoundEffectId = 1;
        Component->SetEnabledState(true);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        if (isFocusSet && !isNativeDestructor) {
            if (ParentAddon is not null) {
                ClearFocusable(ParentAddon);
            }
        }

        base.Dispose(isNativeDestructor);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = new Vector2(Width, Height - 1.0f);
        LabelNode.Size = new Vector2(Width - 32.0f, Height - 3.0f);
        ResizePopupList();
        RebuildPopupList();
    }

    private void MoveListOnScreen() {
        if (ParentAddon is null) return;

        var screenSize = AtkStage.Instance()->ScreenSize;

        var scale = ParentAddon->Scale;
        var scaledListSize = PopupContainerNode.Size * scale;
        if (ScreenPosition.X + scaledListSize.X > screenSize.Width) {
            PopupContainerNode.X += (screenSize.Width - PopupContainerNode.ScreenPosition.X - scaledListSize.X - 4f) / scale;
        }
        else if (ScreenPosition.X < 0) {
            PopupContainerNode.X -= PopupContainerNode.ScreenPosition.X / scale;
        }

        if (PopupContainerNode.ScreenPosition.Y + scaledListSize.Y > screenSize.Height) {
            PopupContainerNode.Y += (screenSize.Height - PopupContainerNode.ScreenPosition.Y - scaledListSize.Y) / scale;
        }
    }

    private void RebuildPopupList() {
        var popupHeight = GetPopupListHeight();

        if (Math.Abs(PopupContainerNode.Height - popupHeight) > 0.1f) {
            var buttonCount = Math.Min(MaxListOptions, Options.Count);

            PopupButtonListNode.Clear();
            foreach (var _ in Enumerable.Range(0, buttonCount)) {
                var buttonNode = new ListButtonNode {
                    Height = 22.0f,
                    String = "DEBUG: Unset",
                };

                PopupButtonListNode.AddNode(buttonNode);
            }

            ResizePopupList();
        }

        PopulatePopupList();
    }

    private void ResizePopupList() {
        PopupContainerNode.Size = new Vector2(Width - 8.0f, GetPopupListHeight());
        PopupContainerNode.Position = new Vector2(4.0f, Height - 3.0f);

        PopupBackgroundNode.Size = PopupContainerNode.Size;
        PopupContainerNode.Position = new Vector2(0.0f, 0.0f);

        PopupScrollbarNode.Size = new Vector2(8.0f, PopupContainerNode.Height - 18.0f);
        PopupScrollbarNode.Position = new Vector2(PopupContainerNode.Width - 14.0f, 4.5f);
        PopupScrollbarNode.UpdateScrollParams(PopupScrollbarNode.Height, Options.Count * 22.0f);

        PopupButtonListNode.Size = new Vector2(PopupBackgroundNode.Width - 24.0f, PopupBackgroundNode.Height - 18.0f);
        PopupButtonListNode.Position = new Vector2(8.0f, 6.0f);
        PopupButtonListNode.RecalculateLayout();
    }

    private void PopulatePopupList() {
        var populateIndex = 0;
        var buttonNodes = PopupButtonListNode.GetNodes<ListButtonNode>().ToList();

        foreach (var entry in Options.Skip(scrollPosition)) {
            if (populateIndex >= buttonNodes.Count) break;

            var buttonNode = buttonNodes[populateIndex++];

            buttonNode.String = GetLabelFunction?.Invoke(entry) ?? "DropDownNode.GetLabelFunction was not set.";
            buttonNode.Selected = SelectedOption?.Equals(entry) ?? false;
            buttonNode.OnClick = () => OnPopupButtonClicked(entry);
        }
    }

    private void OnPopupButtonClicked(T entry) {
        SelectedOption = entry;
        OnOptionSelected?.Invoke(SelectedOption);
        UpdateLabel(entry);
        Collapse();
    }

    private void OnScrollUpdate(int newPosition) {
        scrollPosition = (int)(newPosition / 22.0f);
        PopulatePopupList();
    }

    private void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        scrollPosition += atkEventData->IsScrollUp ? -1 : 1;
        scrollPosition = Math.Clamp(scrollPosition, 0, Math.Max(0, Options.Count - PopupButtonListNode.Nodes.Count));

        PopupScrollbarNode.ScrollPosition = (int)(scrollPosition * 22.0f);
        PopulatePopupList();

        atkEvent->SetEventIsHandled();
    }

    private void UpdateLabel(T? entry) {
        if (entry is null) {
            LabelNode.String = PlaceholderString ?? IDataManager.Get().GetExcelSheet<Addon>().GetRow(1818).Text;
        }
        else {
            LabelNode.String = GetLabelFunction?.Invoke(entry) ?? "DropDownNode.GetLabelFunction was not set.";
        }
    }

    private float GetPopupListHeight() {
        var buttonCount = Math.Min(MaxListOptions, Options.Count);
        return buttonCount * 22.0f + 18.0f;
    }

    /// <summary>
    /// Sets this node as being interactable outside the bounds of the target window.
    /// </summary>
    /// <remarks>
    /// If the addon already has two additional focusable nodes set, this will do nothing.
    /// </remarks>
    public void SetFocusable(AtkUnitBase* addon) {
        foreach (ref var focusableNode in addon->AdditionalFocusableNodes) {
            if (focusableNode.Value is null) {
                focusableNode = PopupContainerNode.ResNode;
                isFocusSet = true;
            }
        }
    }

    /// <summary>
    /// Clears this node as being interactable outside the bounds of the target window.
    /// </summary>
    /// <param name="addon"></param>
    public void ClearFocusable(AtkUnitBase* addon) {
        foreach (ref var focusableNode in addon->AdditionalFocusableNodes) {
            if (focusableNode.Value == PopupContainerNode.ResNode) {
                focusableNode = null;
                isFocusSet = false;
            }
        }
    }

    private SimpleComponentNode PopupContainerNode { get; }
    private SimpleNineGridNode PopupBackgroundNode { get; }
    private ScrollBarNode PopupScrollbarNode { get; }
    private VerticalListNode PopupButtonListNode { get; }

    private int scrollPosition;
    private bool isFocusSet;

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 120)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 2, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 3, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(30, 7, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(39, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(40, 6, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(49, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(50, 4, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(59, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(60, 8, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(69, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(70, 9, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(79, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 10, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(89, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(90, 14, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(99, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(100, 13, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(109, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(110, 11, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(120, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );
        CollapseArrowNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, position: new Vector2(6, 18), rotation: 4.712389f, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, position: new Vector2(6, 17), rotation: 4.712389f, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, position: new Vector2(6, 7), rotation: 0, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, position: new Vector2(6, 6), rotation: 0, alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, position: new Vector2(6, 6), rotation: 0, alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        LabelNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, position: new Vector2(20, 1), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, position: new Vector2(20, 0), alpha: 153, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, position: new Vector2(20, 1), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, position: new Vector2(20, 0), alpha: 153, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, position: new Vector2(20, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, position: new Vector2(0, 1), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, position: new Vector2(0, 0), alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, position: new Vector2(0, 1), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, position: new Vector2(0, 0), alpha: 178, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 120)
            .AddFrame(110, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, position: new Vector2(0, 0), alpha: 255, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }
}
