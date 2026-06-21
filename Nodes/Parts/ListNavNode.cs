using System;
using System.Diagnostics;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Part of <see cref="ListNode{T,TU}"/> used for helping controller nav.
/// </summary>
public sealed unsafe class ListNavNode : SimpleComponentNode {

    /// <summary>
    /// Gets or sets an action to be invoked when this node receives an Upwards Nav.
    /// </summary>
    public Action? OnUpNavReceived { get; set; }

    /// <summary>
    /// Gets or sets an action to be invoked when this node receives a Downwards Nav.
    /// </summary>
    public Action? OnDownNavReceived { get; set; }

    /// <summary>
    /// Constructs a new <see cref="ListNavNode"/>
    /// </summary>
    public ListNavNode() {
        Height = 4.0f;

        AddEvent(AtkEventType.InputReceived, OnInputReceived);
    }

    /// <inheritdoc />
    public override float Height {
        get => base.Height;
        set {
            _ = value;
            base.Height = 4.0f;
        }
    }

    private void OnInputReceived(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is not AtkEventType.InputReceived) return;

        // throttle nav events so it doesn't go hyper crazy.
        if (stopwatch.Elapsed.TotalMilliseconds <= 50) return;

        switch ((InputId)atkEventData->InputData.InputId) {
            case InputId.UP when atkEventData->InputData.State is InputState.Held or InputState.Up:
                OnUpNavReceived?.Invoke();
                stopwatch.Restart();
                break;

            case InputId.DOWN when atkEventData->InputData.State is InputState.Held or InputState.Up:
                OnDownNavReceived?.Invoke();
                stopwatch.Restart();
                break;
        }
    }

    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
}
