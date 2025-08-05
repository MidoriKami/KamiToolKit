using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.TabBar;

public class TabBarNode : SimpleComponentNode {

    private List<TabBarRadioButtonNode> radioButtons = [];

    public TabBarNode() {
        BuildTimelines();
    }

    public void AddTab(SeString label, Action callback) {
        var newButton = new TabBarRadioButtonNode {
            Height = Height, IsVisible = true, Label = label, OnClick = callback,
        };

        newButton.AddEvent(AddonEventType.ButtonClick, data => ClickHandler(data, newButton));

        radioButtons.Add(newButton);
        newButton.AttachNode(this);

        if (radioButtons.Count is 1) {
            newButton.IsSelected = true;
        }

        RecalculateLayout();
    }

    private void ClickHandler(AddonEventData obj, TabBarRadioButtonNode button) {
        foreach (var radioButton in radioButtons) {
            radioButton.IsChecked = false;
            radioButton.IsSelected = false;
        }

        button.IsChecked = true;
        button.IsSelected = true;
    }

    public void RemoveTab(SeString label) {
        var button = radioButtons.FirstOrDefault(button => button.Label == label);
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
