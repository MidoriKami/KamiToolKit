using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
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
public partial class HotbarNode : DragDropNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode KeybindTextNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode ItemCountTextNode { get; }

    /// <summary>
    /// Updates the hotbar slots current state, cost, icon, and various other fields.
    /// </summary>
    public void Update() {
        UpdateSlotAppearance();

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
    /// Sets this hotbar slot from the provided payload information.
    /// </summary>
    public void SetSlot(DragDropPayload payload)
        => SetHotbarSlotFromPayload(payload);

    /// <summary>
    /// Sets this hotbar slot to the specific type and id.
    /// </summary>
    public void SetSlot(DragDropType type, uint id) {
        SetHotbarSlotFromPayload(new DragDropPayload {
            Type = type,
            Int2 = (int) id,
        });
    }

    /// <summary>
    /// Clears the data for this hotbar slot.
    /// </summary>
    public unsafe void ClearSlot() {
        Payload.Clear();
        hotbarState.Ctor();
        hotbarData.Clear();
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
        SetHotbarSlotFromPayload(new DragDropPayload {
            Type = DragDropType.Action,
            Int2 = (int) actionId,
        });
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

        ItemCountTextNode = new TextNode {
            NodeId = 5,
            Position = new Vector2(4.0f, 34.0f),
            Size = new Vector2(40.0f, 12.0f),
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = ColorHelper.GetColor(55),
            AlignmentType = AlignmentType.Right,
            TextFlags = TextFlags.Edge | (TextFlags) 0x8000,
        };
        ItemCountTextNode.AttachNode(this);

        OnRollOver = OnHotbarNodeRollOver;
        OnRollOut = OnHotbarNodeRollOut;
        OnPayloadAccepted = OnHotbarNodePayloadAccepted;
        OnClicked = OnHotbarNodeClicked;
        OnDiscard = OnHotbarNodeDiscard;
        OnBegin = OnDragDropBegin;
        OnEnd = OnDragDropEnd;

        unsafe {
            hotbarData.PopUpHelp.Ctor();
            hotbarData.Clear();
        }

        IGameGui.Get().AgentUpdate += OnAgentUpdate;

        RegisterDebugWindow();
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        base.Dispose(isNativeDestructor);

        IGameGui.Get().AgentUpdate -= OnAgentUpdate;
        UnregisterDebugWindow();
    }

    private void OnAgentUpdate(AgentUpdateFlag agentFlags) {
        if (!agentFlags.HasFlag(AgentUpdateFlag.ActionBarUpdate)) return;

        SetHotbarSlotFromPayload(Payload);
    }

    private void OnHotbarNodeRollOver(DragDropNode thisNode) {
        HideTooltip();

        if (!hotbarData.IsValid) return;

        TextTooltip = hotbarData
            .GetDisplayNameForSlot(hotbarData.ApparentSlotType, hotbarData.ApparentActionId)
            .ToString();

        switch (hotbarData.CommandType) {
            case RaptureHotbarModule.HotbarSlotType.Action:
                ActionTooltip = hotbarData.CommandId;
                break;

            case RaptureHotbarModule.HotbarSlotType.Macro when hotbarData.ApparentSlotType is RaptureHotbarModule.HotbarSlotType.Action:
                ActionTooltip = hotbarData.ApparentActionId;
                break;

            default:
                ActionTooltip = 0;
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

        SetHotbarSlotFromPayload(Payload);
        Update();
    }

    private unsafe void OnHotbarNodeClicked(DragDropNode thisNode) {
        var hotbarModule = RaptureHotbarModule.Instance();
        if (hotbarModule is null) return;

        fixed (RaptureHotbarModule.HotbarSlot* hotbarSlotData = &hotbarData) {
            hotbarModule->ExecuteSlot(hotbarSlotData);
        }
    }

    private void OnHotbarNodeDiscard(DragDropNode thisNode)
        => ClearSlot();

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

    private unsafe void UpdateSlotAppearance() {
        var hotbarModule = RaptureHotbarModule.Instance();
        if (hotbarModule is null) return;

        var isMacro = hotbarData.CommandType is RaptureHotbarModule.HotbarSlotType.Macro;
        var isItem = hotbarData.CommandType is RaptureHotbarModule.HotbarSlotType.Item;

        // Clear hotbar state to get fresh data.
        hotbarState.Ctor();

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
            hotbarData.PopUpHelp.Clear();

            IconId = hotbarState.IconId;

            // IsBackgroundShow tells us we wanna force this slot to be visible.
            IsVisible = hotbarData.IsValid || IsBackgroundShown;

            var isAvailable = hotbarState is { ActionAvailable1: true, ActionAvailable2: true };

            IconNode.IsFaded = !isAvailable && !isMacro;
            IconNode.ShowMacroIcon = isMacro;

            IconNode.ResourceCostVisible = hotbarState.CostType is 2 or 5 or 4; // Mana or GP or CP
            IconNode.ResourceCostValue = hotbarState.CostValue;

            IconNode.CostTextColor = hotbarState.CostType switch {
                2 => CostTextColor.Mana,
                4 => CostTextColor.DoH,
                5 => CostTextColor.DoL,
                _ => CostTextColor.Mana,
            };
            IconNode.IsInvalid = !hotbarState.ActionTargetSatisfied;

            IconNode.ChargeCountVisible = hotbarState.CooldownMode is 3;
            IconNode.ChargeCount = (int) hotbarState.CurrentCharges;
            IconNode.ChargePercent = hotbarState.ChargePercent / 100.0f;

            IconNode.CooldownSecondsVisible = hotbarState.CooldownSeconds is not 0;
            IconNode.CooldownSeconds = hotbarState.CooldownSeconds;

            IconNode.CooldownPercentVisible = hotbarState.CooldownPercent is not 0;
            IconNode.CooldownPercent = hotbarState.CooldownPercent / 100.0f;

            IconNode.IsAnts = hotbarState.DrawAnts;

            ItemCountTextNode.IsVisible = !IconNode.CooldownSecondsVisible && isItem;
            ItemCountTextNode.String = hotbarData.CostTextString;

            KeybindTextNode.String = KeyBind is null ? string.Empty : GetKeybindText(KeyBind);
            KeybindTextNode.IsVisible = KeyBind is not null && hotbarData.IsValid || IsBackgroundShown;
        }
    }

    private unsafe void SetHotbarSlotFromPayload(DragDropPayload payload) {
        var hotbarSlotType = UIGlobals.GetHotbarSlotTypeFromDragDropType(payload.Type);

        IPluginLog.Get().Verbose("HotbarSlot received payload:\n" +
                                 $"Type: {payload.Type}\n" +
                                 $"Int1: {payload.Int1}\n" +
                                 $"Int2: {payload.Int2}\n" +
                                 $"ReferenceIndex: {payload.ReferenceIndex}");


        switch (hotbarSlotType) {
            case RaptureHotbarModule.HotbarSlotType.InventoryItem:

                if (payload.Int1 is not (48 or 49 or 50 or 51)) {
                    IPluginLog.Get().Verbose("Received item from invalid location, skipping setting hotbar.");
                    return;
                }

                var sorterEntry = ItemOrderModule.Instance()->InventorySorter->Items[payload.ReferenceIndex].Value;
                var id = (uint)sorterEntry->Page << 16 | (uint)sorterEntry->Slot & 0xFFFF;

                Payload = payload.Clone();
                hotbarData.Clear();
                hotbarData.Set(hotbarSlotType, id);
                return;

            case RaptureHotbarModule.HotbarSlotType.KeyItem:
                if (payload.Int1 is not 7) {
                    IPluginLog.Get().Verbose("Received key item from invalid location, skipping setting hotbar.");
                    return;
                }

                Payload = payload.Clone();
                hotbarData.Clear();
                hotbarData.Set(hotbarSlotType, (uint) payload.ReferenceIndex);
                return;

            case RaptureHotbarModule.HotbarSlotType.Crystal:
                if (payload.Int1 is not 9) {
                    IPluginLog.Get().Verbose("Received crystal item from invalid location, skipping setting hotbar.");
                    return;
                }

                Payload = payload.Clone();
                hotbarData.Clear();
                hotbarData.Set(hotbarSlotType, (uint) payload.ReferenceIndex);
                return;

            default:
                Payload = payload.Clone();
                hotbarData.Clear();
                hotbarData.Set(hotbarSlotType, (uint) Payload.Int2);
                return;
        }
    }

    private RaptureHotbarModule.HotbarSlot hotbarData;
    private RaptureHotbarModule.HotbarUIIntermediate hotbarState;
    private DateTime? lastClickTime;

    private static HotbarNode? dragSourceNode;
    private static bool settingSourceNodeData;
}
