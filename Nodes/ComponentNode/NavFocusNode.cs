using System;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// A basic component node with callbacks for navigation events.
/// </summary>
public unsafe class NavFocusNode : SimpleComponentNode {

    /// <summary>
    /// Action that is invoked when this node is selected via Return.
    /// </summary>
    public Action? OnSelected { get; set; }

    public NavFocusNode()
        => AddEvent(AtkEventType.InputReceived, Callback);

    private void Callback(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is not AtkEventType.InputReceived) return;

        var inputState = atkEventData->InputData.State;

        switch ((InputId)atkEventData->InputData.InputId) {
            case InputId.OK when inputState is InputState.Down:
                OnSelected?.Invoke();
                break;
        }
    }
}
