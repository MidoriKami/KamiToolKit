using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

public class RadioButtonGroupNode : SimpleComponentNode {

    private List<RadioButtonNode> radioButtons = [];

    public RadioButtonGroupNode() {
        BuildTimelines();
    }

    public ReadOnlySeString? SelectedOption {
        get => radioButtons.FirstOrDefault(button => button.IsSelected)?.SeString;
        set {
            if (value == null)
                return;

            foreach (var radioButton in radioButtons) {
                radioButton.IsChecked = radioButton.SeString == value;
                radioButton.IsSelected = radioButton.SeString == value;
            }

            RecalculateLayout();
        }
    }

    public float VerticalPadding { get; set; } = 2.0f;

    public void AddButton(ReadOnlySeString label, Action callback) {
        var newRadioButton = new RadioButtonNode {
            Height = 16.0f, 
            SeString = label, 
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

    public void RemoveButton(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.SeString == label);
        if (button is null) return;

        button.Dispose();
        radioButtons.Remove(button);
        RecalculateLayout();
    }

    public void Clear() {
        foreach (var node in radioButtons) {
            node.Dispose();
        }

        radioButtons.Clear();
    }

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
}
