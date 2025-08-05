using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

public abstract class ListNode : ComponentNode<AtkComponentBase, AtkUldComponentDataBase>;

/// Note, automatically inserts buttons to fill the set height, please ensure option count is greater than button count.
public abstract class ListNode<T> : ListNode {

    public readonly NineGridNode BackgroundNode;
    public readonly ResNode ContainerNode;
    public readonly ScrollBarNode ScrollBarNode;
    public List<ListButtonNode> Nodes = [];

    protected ListNode() {
        SetInternalComponentType(ComponentType.Base);

        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListB.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(32.0f, 32.0f),
            TopOffset = 10,
            BottomOffset = 12,
            LeftOffset = 10,
            RightOffset = 10,
            IsVisible = true,
        };

        BackgroundNode.AttachNode(this);

        ContainerNode = new ResNode {
            NodeFlags = NodeFlags.Clip, IsVisible = true,
        };

        ContainerNode.AttachNode(this);

        ScrollBarNode = new ScrollBarNode {
            Position = new Vector2(0.0f, 9.0f), Size = new Vector2(8.0f, 0.0f), IsVisible = true, OnValueChanged = OnScrollUpdate,
        };

        ScrollBarNode.AttachNode(this);

        BuildTimelines();

        ContainerNode.SetEventFlags();
        ContainerNode.AddEvent(AddonEventType.MouseWheel, OnMouseWheel);
    }

    public T? SelectedOption { get; set; }

    public List<T>? Options {
        get;
        set {
            field = value;
            RebuildNodeList();
        }
    }

    protected float NodeHeight { get; set; } = 22.0f;

    private int ButtonCount { get; set; }

    public int MaxButtons {
        get;
        set {
            field = value;
            RebuildNodeList();
        }
    } = 5;

    public int CurrentStartIndex { get; set; }

    public Action<T>? OnOptionSelected { get; set; }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;
        ContainerNode.Size = new Vector2(Width - 25.0f, Height);

        foreach (var buttonNode in Nodes) {
            buttonNode.Width = Width - 25.0f;
        }

        ScrollBarNode.X = Width - 17.0f;
    }

    private void OnScrollUpdate(int scrollPosition) {
        var index = scrollPosition / 22.0f;

        CurrentStartIndex = (int)index;
        UpdateNodes();
    }

    private void OnMouseWheel(AddonEventData data) {
        CurrentStartIndex -= data.GetMouseData().WheelDirection;
        UpdateNodes();
        ScrollBarNode.ScrollPosition = (int)(CurrentStartIndex * NodeHeight + 9.0f);

        data.SetHandled();
    }

    private void RebuildNodeList() {
        foreach (var button in Nodes) {
            button.Dispose();
        }
        Nodes.Clear();

        ButtonCount = Math.Min(MaxButtons, Options?.Count ?? 0);

        var height = ButtonCount * NodeHeight + 24.0f;
        Height = height;
        BackgroundNode.Height = height;
        ContainerNode.Height = height;
        ScrollBarNode.Height = height - 23.0f;

        foreach (var index in Enumerable.Range(0, ButtonCount)) {
            var newButton = new ListButtonNode {
                NodeId = (uint)index,
                Size = new Vector2(Width - 25.0f, NodeHeight),
                Position = new Vector2(8.0f, NodeHeight * index + 9.0f),
                IsVisible = true,
                Label = $"Button {index}",
                OnClick = () => OnOptionClick(index),
            };

            Nodes.Add(newButton);
            newButton.AttachNode(ContainerNode);
        }

        if (Options is not null) {
            ScrollBarNode.UpdateScrollParams((int)ScrollBarNode.Height, (int)(Options.Count * NodeHeight + 24.0f));
        }

        UpdateNodes();
    }

    protected virtual void OnOptionClick(int nodeId) {
        if (Options is null) return;

        SelectedOption = Options[nodeId + CurrentStartIndex];
        OnOptionSelected?.Invoke(Options[nodeId + CurrentStartIndex]);

        UpdateSelected();
    }

    private void UpdateSelected() {
        if (Options is null) return;

        foreach (var index in Enumerable.Range(0, ButtonCount)) {
            var option = Options[index + CurrentStartIndex];

            Nodes[index].Selected = SelectedOption?.Equals(option) ?? false;
            Nodes[index].Label = GetLabelForOption(option);
        }
    }

    protected abstract string GetLabelForOption(T option);

    protected void UpdateNodes() {
        if (Options is null) return;
        var maxStartIndex = Options.Count - Nodes.Count;

        var max = Math.Max(0, maxStartIndex);
        CurrentStartIndex = Math.Clamp(CurrentStartIndex, 0, max);
        UpdateSelected();
    }

    public void SelectDefaultOption() {
        if (Options is not null) {
            SelectedOption = Options.First();
        }
    }

    public void Show() {
        IsVisible = true;
        DrawFlags = 0x200000;
    }

    public void Hide() {
        IsVisible = false;
        DrawFlags = 0x100;
    }

    public void Toggle(bool newState) {
        if (newState) {
            Show();
        }
        else {
            Hide();
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
}
