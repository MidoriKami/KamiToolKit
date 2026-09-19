
using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="DragDropNode"/> that has handy accessors for things used to represent a hotbar slot.
/// </summary>
public class HotbarNode : DragDropNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode KeybindTextNode { get; }

    /// <summary>
    /// Updates the hotbar slots current state, cost, icon, and various other fields.
    /// </summary>
    public unsafe void Update() {
        var hotbarModule = RaptureHotbarModule.Instance();
        if (hotbarModule is null) return;

        hotbarState.Ctor();

        // Update hotbar data each frame, this is probably wasteful,
        // but we're still triggering like 1/10th the updates native does, so sue me.
        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(Payload.Type), (uint) Payload.Int2);

        var isMacro = hotbarData.CommandType is RaptureHotbarModule.HotbarSlotType.Macro;

        fixed (RaptureHotbarModule.HotbarSlot* data = &hotbarData)
        fixed (RaptureHotbarModule.HotbarUIIntermediate* state = &hotbarState)
        {
            RaptureHotbarModule.HotbarSlotType outType;
            uint outActionId;
            ushort unkC4;

            RaptureHotbarModule.GetSlotAppearance(&outType, &outActionId, &unkC4, hotbarModule, data);
            hotbarData.ApparentActionId = outActionId;
            hotbarData.ApparentSlotType = outType;

            RaptureHotbarModule.Instance()->PrepareSlotForRender(data, state);

            IconId = hotbarState.IconId;

            // IsBackgroundShow tells us we wanna force this slot to be visible.
            IsVisible = hotbarData.IsValid || IsBackgroundShown;

            var isAvailable = hotbarState is { ActionAvailable1: true, ActionAvailable2: true };

            IconNode.IsFaded = !isAvailable && !isMacro;
            IconNode.ShowMacroIcon = isMacro;

            IconNode.ResourceCostVisible = hotbarState.CostType is 2 or 5; // Mana or GP
            IconNode.ResourceCostValue = hotbarState.CostValue;

            IconNode.CostTextColor = hotbarState.CostType switch {
                2 => CostTextColor.Mana,
                5 => CostTextColor.DoL,
                _ => CostTextColor.Mana,
            };
            IconNode.IsInvalid = !hotbarState.ActionTargetSatisfied;

            IconNode.ChargeCountVisible = hotbarState.CooldownMode is 3;
            IconNode.ChargeCount = hotbarState.CurrentCharges;
            IconNode.ChargePercent = hotbarState.ChargePercent / 100.0f;

            IconNode.CooldownSecondsVisible = hotbarState.CooldownSeconds is not 0;
            IconNode.CooldownSeconds = hotbarState.CooldownSeconds;

            IconNode.CooldownPercentVisible = hotbarState.CooldownPercent is not 0;
            IconNode.CooldownPercent = hotbarState.CooldownPercent / 100.0f;

            IconNode.IsAnts = hotbarState.DrawAnts;

            KeybindTextNode.String = KeyBind is null ? string.Empty : GetKeybindText(KeyBind);
            KeybindTextNode.IsVisible = KeyBind is not null && hotbarData.IsValid || IsBackgroundShown;
        }

        TryProcessKeybind();

        if (lastClickTime is not null && DateTime.UtcNow - lastClickTime > TimeSpan.FromMilliseconds(330)) {
            IconNode.IconExtras.Timeline?.PlayAnimation(4);
            lastClickTime = null;
        }
    }

    /// <summary>
    /// Gets or sets the keybind that will activate this slot.
    /// </summary>
    public KeySetting? KeyBind { get; set; }

    /// <summary>
    /// Gets whether this slot is empty.
    /// </summary>
    public bool IsEmpty => Payload.Type is 0 or DragDropType.Nothing;

    /// <summary>
    /// Sets this hotbar slot to the specific type and id.
    /// </summary>
    public void SetSlot(DragDropType type, uint id) {
        Payload.Type = type;
        Payload.Int2 = (int) id;

        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(Payload.Type), (uint) Payload.Int2);
    }

    /// <summary>
    /// Sets this hotbar slot to the specific type and id.
    /// </summary>
    public void SetSlot(DragDropType payloadType, int payloadInt)
        => SetSlot(payloadType, (uint)payloadInt);

    /// <summary>
    /// Function that is called when the associated <see cref="KeyBind"/> is pressed.
    /// </summary>
    protected virtual void OnKeybindPressed() {
        OnHotbarNodeClicked(this);
        IconNode.IconExtras.Timeline?.PlayAnimation(3, true);
        lastClickTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets this hotbar slot to the specified action.
    /// </summary>
    public void SetAction(uint actionId) {
        Payload.Type = DragDropType.Action;
        Payload.Int2 = (int) actionId;

        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(Payload.Type), (uint) Payload.Int2);
    }

    /// <inheritdoc />
    public HotbarNode() {
        KeybindTextNode = new TextNode {
            NodeId = 4,
            Position = new Vector2(1.0f, -6.0f),
            Size = new Vector2(50.0f, 20.0f),
            FontType = FontType.MiedingerMed,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = new Vector4(0.200f, 0.200f, 0.200f, 1.000f),
            TextFlags = TextFlags.Edge | TextFlags.Ellipsis | (TextFlags) 0x8000,
        };
        KeybindTextNode.AttachNode(this);

        OnRollOver = OnHotbarNodeRollOver;
        OnRollOut = OnHotbarNodeRollOut;
        OnPayloadAccepted = OnHotbarNodePayloadAccepted;
        OnClicked = OnHotbarNodeClicked;
        OnDiscard = OnHotbarNodeDiscard;
        OnBegin = OnDragDropBegin;
        OnEnd = OnDragDropEnd;
    }

    private void OnHotbarNodeRollOver(DragDropNode thisNode) {
        HideTooltip();

        if (!hotbarData.IsValid) return;

        switch (hotbarData.CommandType) {
            case RaptureHotbarModule.HotbarSlotType.Action:
                ActionTooltip = hotbarData.CommandId;
                TextTooltip = string.Empty;
                break;

            case RaptureHotbarModule.HotbarSlotType.Macro:
                TextTooltip = hotbarData.PopUpHelp.AsReadOnlySeString();

                if (hotbarData.ApparentSlotType is RaptureHotbarModule.HotbarSlotType.Action) {
                    ActionTooltip = hotbarData.ApparentActionId;
                }
                break;

            default:
                ActionTooltip = 0;
                TextTooltip = string.Empty;
                break;
        }

        ShowTooltip();
    }

    private void OnHotbarNodeRollOut(DragDropNode thisNode) {
        HideTooltip();
    }

    private void OnHotbarNodePayloadAccepted(DragDropNode thisNode, DragDropPayload payload) {
        if (settingSourceNodeData) return;

        // If source is another KTK Node
        if (dragSourceNode is not null) {

            // And we are not empty, we need to swap our slots.
            if (hotbarData.IsValid) {
                var sourcePayload = dragSourceNode.Payload.Clone();

                // Call OnPayloadAccepted to trigger any down-stream events that are listening for payload change.
                try {
                    settingSourceNodeData = true;
                    dragSourceNode.OnPayloadAccepted?.Invoke(dragSourceNode, Payload);
                }

                // Ensure this gets cleared, or this'll prevent all HotbarNodes from accepting payloads.
                finally {
                    settingSourceNodeData = false;
                }

                dragSourceNode.Payload = Payload.Clone();
                Payload = sourcePayload;
            }

            // If we are empty, take the sources payload, and tell it to discard what it has
            else {
                Payload = dragSourceNode.Payload.Clone();
                dragSourceNode.OnDiscard?.Invoke(dragSourceNode);
            }

            dragSourceNode.Update();
        }

        // Source is a vanilla node
        else {

            // If source is a native slot, eventually write this code to get the HotbarSlot* and clear it
            // But too lazy for that right now.

            Payload = payload.Clone();
        }

        Update();
    }

    private unsafe void OnHotbarNodeClicked(DragDropNode thisNode) {
        var hotbarModule = RaptureHotbarModule.Instance();
        if (hotbarModule is null) return;

        fixed (RaptureHotbarModule.HotbarSlot* hotbarSlotData = &hotbarData) {
            hotbarModule->ExecuteSlot(hotbarSlotData);
        }
    }

    private unsafe void OnHotbarNodeDiscard(DragDropNode thisNode) {
        Payload.Clear();

        hotbarState.Ctor();

        hotbarData.Set(RaptureHotbarModule.HotbarSlotType.Empty, 0);
    }

    private void OnDragDropBegin(DragDropNode node) {
        if (node is not HotbarNode hotbarNode) return;

        dragSourceNode = hotbarNode;
    }

    private void OnDragDropEnd(DragDropNode node) {
        dragSourceNode = null;
    }

    private unsafe void TryProcessKeybind() {
        if (ICondition.Get().Any(ConditionFlag.OccupiedInQuestEvent)) return;
        if (KeyBind is not { Key: not SeVirtualKey.NO_KEY } keyBind) return;
        if (RaptureAtkModule.Instance()->IsTextInputActive()) return;

        var keyStateService = IKeyState.Get();
        if (!keyStateService.IsVirtualKeyValid((int)keyBind.Key)) return;

        // Main key isn't pressed
        if (!keyStateService[(int)keyBind.Key]) return;

        // Only allow one modifier key, with priority Ctrl -> Alt -> Shift
        VirtualKey? modifierKey = keyBind.KeyModifier switch {
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Ctrl) => VirtualKey.CONTROL,
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Alt) => VirtualKey.MENU,
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Shift) => VirtualKey.SHIFT,
            _ => null,
        };

        // If modifier is required
        if (modifierKey is { } modifier) {

            // But isn't valid, return.
            if (!keyStateService.IsVirtualKeyValid(modifier)) {
                return;
            }

            // Or isn't pressed, return.
            if (!keyStateService[modifier]) {
                return;
            }
        }

        // Keybind doesn't use a modifier, check if any modifier is present and ignore if it is pressed.
        else if (modifierKey is null) {

            // If control is valid and is pressed, return
            if (keyStateService.IsVirtualKeyValid(VirtualKey.CONTROL)) {
                if (keyStateService[VirtualKey.CONTROL]) {
                    return;
                }
            }

            // If alt is valid and is pressed, return
            if (keyStateService.IsVirtualKeyValid(VirtualKey.MENU)) {
                if (keyStateService[VirtualKey.MENU]) {
                    return;
                }
            }

            // If shift is valid and is pressed, return
            if (keyStateService.IsVirtualKeyValid(VirtualKey.SHIFT)) {
                if (keyStateService[VirtualKey.SHIFT]) {
                    return;
                }
            }
        }

        // Modifier (if any), and main key is pressed here.

        // Clear the pressed key, leave modifiers pressed.
        keyStateService[(int)keyBind.Key] = false;

        OnKeybindPressed();
    }

    /// <summary>
    /// Gets the displayed string used to represent a keybind.
    /// </summary>
    public static ReadOnlySeString GetKeybindText(KeySetting? keybind) {
        if (keybind is not {} keyBind) return string.Empty;

        ReadOnlySeString? modifierKey = keyBind.KeyModifier switch {
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Ctrl) => "¢",
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Alt) => "ª",
            _ when keyBind.KeyModifier.HasFlag(KeyModifierFlag.Shift) => "§",
            _ => null,
        };

        return $"{modifierKey}{(char)keyBind.Key}";
    }

    private RaptureHotbarModule.HotbarSlot hotbarData;
    private RaptureHotbarModule.HotbarUIIntermediate hotbarState;
    private DateTime? lastClickTime;

    private static HotbarNode? dragSourceNode;
    private static bool settingSourceNodeData;
}
