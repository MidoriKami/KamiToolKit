using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Nodes;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Node representing a set of radio buttons.
/// </summary>
public class RadioButtonGroupNode : ResNode {

    /// <summary>
    /// Gets or sets the selection option via label.
    /// </summary>
    public ReadOnlySeString? SelectedOption {
        get => radioButtons.FirstOrDefault(button => button.IsSelected)?.String;
        set {
            if (value == null)
                return;

            foreach (var radioButton in radioButtons) {
                radioButton.IsChecked = radioButton.String == value;
                radioButton.IsSelected = radioButton.String == value;
            }

            RecalculateLayout();
        }
    }

    /// <summary>
    /// Gets or sets the vertical padding used between radio buttons.
    /// </summary>
    public float VerticalPadding { get; set; } = 2.0f;

    /// <summary>
    /// Adds a radio button by name, and registers the provided callback when the button is triggered.
    /// </summary>
    public void AddButton(ReadOnlySeString label, Action callback) {
        var newRadioButton = new RadioButtonNode {
            Height = 16.0f,
            String = label,
            Callback = callback,
        };

        newRadioButton.AddEvent(AtkEventType.ButtonClick, () => ClickHandler(newRadioButton));

        radioButtons.Add(newRadioButton);
        newRadioButton.AttachNode(this);

        if (radioButtons.Count is 1) {
            newRadioButton.IsChecked = true;
            newRadioButton.IsSelected = true;
        }

        RecalculateLayout();
    }

    /// <summary>
    /// Removes the button via the specified label.
    /// </summary>
    public void RemoveButton(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.Dispose();
        radioButtons.Remove(button);
        RecalculateLayout();
    }

    /// <summary>
    /// Removes all radio buttons from this node.
    /// </summary>
    public void Clear() {
        foreach (var node in radioButtons) {
            node.Dispose();
        }

        radioButtons.Clear();
    }

    public RadioButtonGroupNode()
        => BuildTimelines();

    private void RecalculateLayout() {
        var yPosition = 0.0f;

        foreach (var index in Enumerable.Range(0, radioButtons.Count)) {
            var button = radioButtons[index];

            button.Y = yPosition;
            yPosition += button.Height + VerticalPadding;
        }

        Height = yPosition;
    }

    private void ClickHandler(RadioButtonNode selectedButton) {
        foreach (var radioButton in radioButtons) {
            radioButton.IsChecked = false;
            radioButton.IsSelected = false;
        }

        selectedButton.IsChecked = true;
        selectedButton.IsSelected = true;
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 19)
            .AddLabel(1, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );
    }

    private readonly List<RadioButtonNode> radioButtons = [];
}
