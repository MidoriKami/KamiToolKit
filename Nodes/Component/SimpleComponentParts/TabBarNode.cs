using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes.SimpleComponentParts;

public class TabBarNode : SimpleComponentNode {

    private List<TabBarRadioButtonNode> radioButtons = [];

    public TabBarNode() {
        BuildTimelines();
    }

    public void AddTab(ReadOnlySeString label, Action callback, bool isEnabled = true) {
        var newButton = new TabBarRadioButtonNode {
            Height = Height, 
            SeString = label, 
            OnClick = callback,
            IsEnabled = isEnabled,
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

    public void DisableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.SeString == label);
        if (button is null) return;

        button.IsEnabled = false;
        button.MultiplyColor = new Vector3(0.6f, 0.6f, 0.6f);
    }

    public void EnableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.SeString == label);
        if (button is null) return;

        button.IsEnabled = true;
        button.MultiplyColor = Vector3.One;
    }

    public void ToggleTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.SeString == label);
        if (button is null) return;

        button.IsEnabled = !button.IsEnabled;

        if (button.IsEnabled) {
            button.MultiplyColor = Vector3.One;
        }
        else {
            button.MultiplyColor = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }
    
    public void RemoveTab(ReadOnlySeString label) {
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
        var step = Width / radioButtons.Count;

        foreach (var index in Enumerable.Range(0, radioButtons.Count)) {
            var button = radioButtons[index];

            button.Width = step + 5.0f;
            button.X = step * index - 5.0f;
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
}
