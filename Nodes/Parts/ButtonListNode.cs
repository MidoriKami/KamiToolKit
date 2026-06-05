using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.Simple;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <inheritdoc/>
public abstract class ListNode : ResNode;

/// <summary>
/// Custom implementation of the games button lists used for <see cref="DropDownNode{T,TU}"/>, not intended for external use.
/// </summary>
/// <remarks>
/// Automatically inserts buttons to fill the set height, please ensure option count is greater than button count.
/// </remarks>
public abstract unsafe class ButtonListNode<T> : ListNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ResNode ContainerNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollBarNode ScrollBarNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IEnumerable<ListButtonNode> ButtonNodes => nodes.AsReadOnly();

    /// <summary>
    /// Gets or sets the node that will be highlighted as selected.
    /// </summary>
    public T? SelectedOption {
        get;
        set {
            field = value;
            UpdateSelected();
        }
    }

    /// <summary>
    /// Gets or sets the list of button options.
    /// </summary>
    /// <remarks>
    /// This will rebuild the node list.
    /// </remarks>
    public List<T>? Options {
        get;
        set {
            field = value;
            RebuildNodeList();
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of buttons to display at once.
    /// </summary>
    public int MaxButtons {
        get;
        set {
            field = value;
            RebuildNodeList();
        }
    } = 5;

    /// <summary>
    /// Gets or sets the action to be called when an option is clicked on.
    /// </summary>
    public Action<T>? OnOptionSelected { get; set; }

    /// <summary>
    /// Forces the scrollbar to update its scrolling parameters.
    /// </summary>
    public void RecalculateScrollParams() {
        if (Options is not null) {
            ScrollBarNode.UpdateScrollParams((int)ScrollBarNode.Height, (int)(Options.Count * NodeHeight));
        }
    }

    /// <summary>
    /// Sets the first option as the selected option.
    /// </summary>
    public void SelectDefaultOption() {
        if (Options is not null && Options.Count > 0) {
            SelectedOption = Options.First();
        }
    }

    /// <summary>
    /// Shows this node.
    /// </summary>
    public void Show() {
        IsVisible = true;
        AddDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null) {
            SetFocusable(ParentAddon);
        }
    }

    /// <summary>
    /// Hides this node.
    /// </summary>
    public void Hide() {
        IsVisible = false;
        RemoveDrawFlags(DrawFlags.RenderOnTop);

        if (ParentAddon is not null) {
            ClearFocusable(ParentAddon);
        }
    }

    /// <summary>
    /// Toggles this nodes visibility.
    /// </summary>
    /// <param name="newState"></param>
    public void Toggle(bool newState) {
        if (newState) {
            Show();
        }
        else {
            Hide();
        }
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
                focusableNode = ResNode;
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
            if (focusableNode.Value == ResNode) {
                focusableNode = null;
                isFocusSet = false;
            }
        }
    }

    protected ButtonListNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListB.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 32.0f),
            TopOffset = 10,
            BottomOffset = 12,
            LeftOffset = 10,
            RightOffset = 10,
        };
        BackgroundNode.AttachNode(this);

        ContainerNode = new ResNode {
            NodeFlags = NodeFlags.Visible | NodeFlags.Clip,
        };
        ContainerNode.AttachNode(this);

        ScrollBarNode = new ScrollBarNode {
            Position = new Vector2(0.0f, 9.0f),
            Size = new Vector2(8.0f, 0.0f),
            OnValueChanged = OnScrollUpdate,
            HideWhenDisabled = true,
        };
        ScrollBarNode.AttachNode(this);

        BuildTimelines();

        ContainerNode.AddEvent(AtkEventType.MouseWheel, OnMouseWheel);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;
        ContainerNode.Size = new Vector2(Width - 25.0f, Height);

        foreach (var buttonNode in nodes) {
            buttonNode.Width = Width - 25.0f;
        }

        ScrollBarNode.X = Width - 17.0f;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            if (isFocusSet && !isNativeDestructor) {
                if (ParentAddon is not null) {
                    ClearFocusable(ParentAddon);
                }
            }

            base.Dispose(disposing, isNativeDestructor);
        }
    }

    protected float NodeHeight { get; set; } = 22.0f;

    protected int CurrentStartIndex { get; set; }

    protected abstract string GetLabelForOption(T option);

    protected virtual void OnOptionClick(int nodeId) {
        if (Options is null) return;

        SelectedOption = Options[nodeId + CurrentStartIndex];
        OnOptionSelected?.Invoke(Options[nodeId + CurrentStartIndex]);

        UpdateSelected();
    }

    private void UpdateNodes() {
        if (Options is null) return;
        var maxStartIndex = Options.Count - nodes.Count;

        var max = Math.Max(0, maxStartIndex);
        CurrentStartIndex = Math.Clamp(CurrentStartIndex, 0, max);
        UpdateSelected();
    }

    private void OnScrollUpdate(int scrollPosition) {
        var index = scrollPosition / 22.0f;

        CurrentStartIndex = (int)index;
        UpdateNodes();
    }

    private void OnMouseWheel(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        CurrentStartIndex -= atkEventData->MouseData.WheelDirection;
        UpdateNodes();
        ScrollBarNode.ScrollPosition = (int)(CurrentStartIndex * NodeHeight + 9.0f);

        atkEvent->SetEventIsHandled();
    }

    private void RebuildNodeList() {
        foreach (var button in nodes) {
            button.DetachNode();
            button.Dispose();
        }
        nodes.Clear();

        buttonCount = Math.Min(MaxButtons, Options?.Count ?? 0);

        var height = buttonCount * NodeHeight + 24.0f;
        Height = height;
        BackgroundNode.Height = height;
        ContainerNode.Height = height;
        ScrollBarNode.Height = height - 23.0f;

        foreach (var index in Enumerable.Range(0, buttonCount)) {
            var newButton = new ListButtonNode {
                NodeId = (uint)index,
                Size = new Vector2(Width - 25.0f, NodeHeight),
                Position = new Vector2(8.0f, NodeHeight * index + 9.0f),

                String = $"Button {index}",
                OnClick = () => OnOptionClick(index),
            };

            nodes.Add(newButton);
            newButton.AttachNode(ContainerNode);
        }

        RecalculateScrollParams();
        UpdateNodes();
    }

    private void UpdateSelected() {
        if (Options is null) return;

        foreach (var index in Enumerable.Range(0, buttonCount)) {
            var option = Options[index + CurrentStartIndex];

            nodes[index].Selected = SelectedOption?.Equals(option) ?? false;
            nodes[index].String = GetLabelForOption(option);
        }
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 29)
            .AddLabel(1, 17, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 18, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 7, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );
    }

    private readonly List<ListButtonNode> nodes = [];
    private bool isFocusSet;
    private int buttonCount;
}
