using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Services.Timelines;
using KamiToolKit.UI.Nodes.ComponentNode;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// Custom implementation of a tab bar.
/// </summary>
public class TabBarNode : ResNode, ITabBarNode {

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

    internal IReadOnlyList<TabBarRadioButtonNode> Buttons => radioButtons;

    /// <inheritdoc />
    public ICollection<TabBarEntry> InitialEntries {
        init {
            foreach (var tabBarEntry in value) {
                AddTab(tabBarEntry);
            }
        }
    }

    /// <inheritdoc />
    public void SelectTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        ClickHandler(button);
    }

    /// <inheritdoc />
    public void DisableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.IsEnabled = false;
        button.MultiplyColor = new Vector3(0.6f, 0.6f, 0.6f);
    }

    /// <inheritdoc />
    public void EnableTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.IsEnabled = true;
        button.MultiplyColor = Vector3.One;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void RemoveTab(ReadOnlySeString label) {
        var button = radioButtons.FirstOrDefault(button => button.String == label);
        if (button is null) return;

        button.Dispose();
        radioButtons.Remove(button);
        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc />
    public void Clear() {
        foreach (var node in radioButtons) {
            node.Dispose();
        }

        radioButtons.Clear();
        UpdateLayout();
        UpdateNavigation();
    }

    /// <summary>
    /// Constructs a new <see cref="TabBarNode"/>
    /// </summary>
    public TabBarNode() {
        BuildTimelines();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        UpdateLayout();
    }

    /// <inheritdoc />
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

        UpdateLayout();
        UpdateNavigation();
    }

    /// <inheritdoc />
    public void AddTab(ReadOnlySeString label, Action? callback = null, ReadOnlySeString? tooltip = null, bool isEnabled = true) {
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

        UpdateLayout();
        UpdateNavigation();
    }

    private void ClickHandler(TabBarRadioButtonNode button) {
        foreach (var radioButton in radioButtons) {
            radioButton.IsChecked = false;
            radioButton.IsSelected = false;
        }

        button.IsChecked = true;
        button.IsSelected = true;
    }

    /// <inheritdoc/>
    public override void UpdateNavigation() => UpdateNavigationFromRoot();

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var button in radioButtons) {
            context.TryAssignIndex(button);
        }

        for (var index = 0; index < radioButtons.Count; index++) {
            var button = radioButtons[index];
            var previous = radioButtons[index is 0 ? radioButtons.Count - 1 : index - 1];
            var next = radioButtons[index == radioButtons.Count - 1 ? 0 : index + 1];

            context.TrySetLink(button, ControllerNavigationDirection.Left, previous);
            context.TrySetLink(button, ControllerNavigationDirection.Right, next);
        }

        NavigationBoundary = radioButtons.Count is 0
            ? ControllerNavigationBoundary.Empty
            : new ControllerNavigationBoundary(radioButtons, radioButtons, [radioButtons[0]], [radioButtons[^1]]);
    }


    private void UpdateLayout() {
        if (radioButtons.Count is 0) return;
        if (Width is 0) return;
        if (Height is 0) return;

        var step = Width / radioButtons.Count;
        foreach (var index in Enumerable.Range(0, radioButtons.Count)) {
            var button = radioButtons[index];

            // Offset value squishes them together because texture doesn't fill size.
            button.Width = step + 5.0f;
            button.X = step * index - 2.5f;
            button.Height = Height;

            // But tweak first and last a little so it isn't weird.
            if (index is 0) {
                button.X = 0.0f;
                button.Width = step + 2.5f;
            }
            else if (index == radioButtons.Count - 1) {
                button.Width = step + 2.5f;
            }

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
