using System;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// A basic component node with callbacks for navigation events.
/// </summary>
public unsafe class NavFocusNode : SimpleComponentNode, INavFocusNode {

    /// <inheritdoc />
    public Action? OnSelected { get; set; }

    /// <inheritdoc />
    public Action? OnHoverStart { get; set; }

    /// <inheritdoc />
    public Action? OnHoverEnd { get; set; }

    /// <summary>
    /// Constructs a new <see cref="NavFocusNode"/>.
    /// </summary>
    public NavFocusNode()
        => AddEvent(AtkEventType.InputReceived, Callback);

    private void Callback(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is not AtkEventType.InputReceived) return;

        var inputState = atkEventData->InputData.State;

        switch ((InputId)atkEventData->InputData.InputId) {
            case InputId.OK when inputState is InputState.Down:
                OnSelected?.Invoke();
                break;

            case InputId.DOWN or InputId.UP when inputState is InputState.Down:
                OnHoverEnd?.Invoke();
                break;

            case InputId.DOWN or InputId.UP when inputState is InputState.Up:
                OnHoverStart?.Invoke();
                break;
        }
    }
}