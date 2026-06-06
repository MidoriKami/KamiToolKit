using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <inheritdoc/>
public abstract class ButtonListNode : SimpleComponentNode;

/// <summary>
/// Custom implementation of the games button lists used for <see cref="DropDownNode{T,TU}"/>, not intended for external use.
/// </summary>
/// <remarks>
/// Automatically inserts buttons to fill the set height, please ensure option count is greater than button count.
/// </remarks>
public abstract unsafe class ButtonListNode<T> : ButtonListNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ScrollingNode<VerticalListNode> ScrollingListNode { get; }

    /// <summary>
    /// Gets or sets the node that will be highlighted as selected.
    /// </summary>
    public T? SelectedOption { get; set; }

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
    /// Gets or sets the action to be called when an option is clicked on.
    /// </summary>
    public Action<T>? OnOptionSelected { get; set; }

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

        ScrollingListNode = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                FirstItemSpacing = 2.0f,
                ItemSpacing = 3.0f,
                FitContents = true,
            },
            AutoHideScrollBar = true,
        };
        ScrollingListNode.AttachNode(this);

        BuildTimelines();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;

        ScrollingListNode.Size = new Vector2(Width - 8.0f, Height - 14.0f);
        ScrollingListNode.Position = new Vector2(2.0f, 4.0f);

        ScrollingListNode.ContentNode.Position = new Vector2(4.0f, 0.0f);
        ScrollingListNode.RecalculateSizes();
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

    protected float ButtonNodeHeight { get; set; } = 22.0f;

    protected abstract string GetLabelForOption(T option);

    protected virtual void OnOptionClick(ListButtonNode listButton, T option) {
        if (Options is null) return;

        selectedButtonNode?.Selected = false;
        selectedButtonNode = listButton;
        selectedButtonNode.Selected = true;

        SelectedOption = option;
        OnOptionSelected?.Invoke(SelectedOption);
    }

    private void RebuildNodeList() {
        ScrollingListNode.ContentNode.Clear();

        foreach (var option in Options ?? []) {
            var newButton = new ListButtonNode {
                Size = new Vector2(Width - 16.0f, ButtonNodeHeight),
                String = GetLabelForOption(option),
            };

            newButton.OnClick = () => OnOptionClick(newButton, option);
            ScrollingListNode.ContentNode.AddNode(newButton);
        }

        ScrollingListNode.RecalculateSizes();
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

    private ListButtonNode? selectedButtonNode;
    private bool isFocusSet;
}
