using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Custom implementation of a tab bar.
/// </summary>
public class TabBarNode : ResNode, IControllerNavigable {

    /// <inheritdoc/>
    public int NavIndex { get; set; }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

    public ICollection<TabBarEntry> InitialEntries {
        init {
            foreach (var tabBarEntry in value) {
                AddTab(tabBarEntry);
            }
        }
    }

    /// <summary>
    /// Selects the tab matching the given label.
    /// </summary>
    public void SelectTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        ClickHandler(button);
    }

    /// <summary>
    /// Disables a tab matching the given label, the tab won't be selectable or interactable.
    /// </summary>
    public void DisableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.IsEnabled = false;
        button.MultiplyColor = new Vector3(0.6f, 0.6f, 0.6f);
    }

    /// <summary>
    /// Enables a tab matching the given label.
    /// </summary>
    public void EnableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.IsEnabled = true;
        button.MultiplyColor = Vector3.One;
    }

    /// <summary>
    /// Toggles a tab matching the given label's enabled/disabled state.
    /// </summary>
    /// <param name="label"></param>
    public void ToggleTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.IsEnabled = !button.IsEnabled;

        if (button.IsEnabled) {
            button.MultiplyColor = Vector3.One;
        }
        else {
            button.MultiplyColor = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }

    /// <summary>
    /// Removes a tab matching the given label.
    /// </summary>
    /// <param name="label"></param>
    public void RemoveTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.Dispose();
        radioButtons.Remove(button);
        RecalculateLayout();
    }

    /// <summary>
    /// Clear all tab nodes.
    /// </summary>
    public void Clear() {
        foreach (var node in radioButtons) {
            node.Dispose();
        }

        radioButtons.Clear();
    }

    public TabBarNode() {
        BuildTimelines();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        RecalculateLayout();
    }

    public void AddTab(TabBarEntry entry) {
        var newButton = new TabBarRadioButtonNode {
            Height = Height,
            String = entry.Label,
            TextId = entry.TextId,
            SheetType = entry.SheetType,
            OnClick = entry.OnClick,
            IsEnabled = true,
            TextTooltip = entry.Tooltip ?? string.Empty,
            MultiplyColor = Vector3.One,
        };

        newButton.AddEvent(AtkEventType.ButtonClick, () => ClickHandler(newButton));

        radioButtons.Add(newButton);
        newButton.AttachNode(this);

        if (radioButtons.Count is 1) {
            newButton.IsSelected = true;
        }

        RecalculateLayout();
    }

    public void AddTab(ReadOnlySeString label, Action callback, ReadOnlySeString? tooltip = null, bool isEnabled = true) {
        var newButton = new TabBarRadioButtonNode {
            Height = Height,
            String = label,
            OnClick = callback,
            IsEnabled = isEnabled,
            TextTooltip = tooltip ?? string.Empty,
            MultiplyColor = isEnabled ? Vector3.One : new Vector3(0.6f, 0.6f, 0.6f),
        };

        newButton.AddEvent(AtkEventType.ButtonClick, () => ClickHandler(newButton));

        radioButtons.Add(newButton);
        newButton.AttachNode(this);

        if (radioButtons.Count is 1) {
            newButton.IsSelected = true;
        }

        RecalculateLayout();
    }

    private void ClickHandler(TabBarRadioButtonNode button) {
        foreach (var radioButton in radioButtons) {
            radioButton.IsChecked = false;
            radioButton.IsSelected = false;
        }

        button.IsChecked = true;
        button.IsSelected = true;
    }

    private void RecalculateLayout() {
        var step = Width / radioButtons.Count;

        foreach (var index in Enumerable.Range(0, radioButtons.Count)) {
            var button = radioButtons[index];

            button.Width = step + 5.0f;
            button.X = step * index - 5.0f;
            button.Height = Height;

            button.NavIndex = NavIndex + index;

            if (NavIndex is not 0) {
                if (index is 0) {
                    button.NavLeft = radioButtons.Count - 1 + NavIndex;
                }
                else {
                    button.NavLeft = index - 1 + NavIndex;
                }

                if (index == radioButtons.Count - 1) {
                    button.NavRight = NavIndex;
                }
                else {
                    button.NavRight = index + 1 + NavIndex;
                }
            }

            button.NavUp = NavUp;
            button.NavDown = NavDown;
        }
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 20)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );
    }

    private readonly List<TabBarRadioButtonNode> radioButtons = [];
}
