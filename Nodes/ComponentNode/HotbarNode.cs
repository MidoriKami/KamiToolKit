
using System.Drawing;
using System.Numerics;
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

        hotbarState = new Experimental.HotbarUiIntermediate {
            PopUpHelpText = null,
        };

        var isMacro = hotbarData.CommandType is RaptureHotbarModule.HotbarSlotType.Macro;

        fixed (RaptureHotbarModule.HotbarSlot* data = &hotbarData)
        fixed (Experimental.HotbarUiIntermediate* state = &hotbarState)
        {
            RaptureHotbarModule.HotbarSlotType outType;
            uint outActionId;
            ushort unkC4;

            RaptureHotbarModule.GetSlotAppearance(&outType, &outActionId, &unkC4, hotbarModule, data);
            hotbarData.ApparentActionId = outActionId;
            hotbarData.ApparentSlotType = outType;

            Experimental.UpdateHotbarSlotIntermediateData?.Invoke(RaptureHotbarModule.Instance(), data, state);

            IconId = hotbarState.IconId;

            var isAvailable = hotbarState.ActionAvailable1 || hotbarState.ActionAvailable2;

            IsAvailable = isAvailable || isMacro;
            ShowMacroIcon = isMacro;

            ShowResourceCost = hotbarState.CostType is 2 or 5; // Mana or GP
            ResourceCost = hotbarState.CostValue;
            CostTextColor = hotbarState.CostType switch {
                2 => CostTextColor.Mana,
                5 => CostTextColor.DoL,
                _ => CostTextColor.Mana,
            };

            ShowChargeCount = hotbarState.CooldownMode is 3;
            ChargeCount = hotbarState.CurrentCharges;
            ChargePercent = hotbarState.ChargePercent / 100.0f;

            ShowCooldownSeconds = hotbarState.CooldownSeconds is not 0;
            CooldownSeconds = hotbarState.CooldownSeconds;

            ShowCooldownPercent = hotbarState.CooldownPercent is not 0;
            CooldownPercent = hotbarState.CooldownPercent / 100.0f;

            if (KeyBind is not null) {
                KeybindTextNode.String = GetKeybindText(KeyBind);
            }

            KeybindTextNode.IsVisible = KeyBind is not null && !hotbarData.IsEmpty || IsBackgroundShown;
        }

        TryProcessKeybind();
    }

    /// <summary>
    /// Gets or sets whether this action is available for use.
    /// </summary>
    public bool IsAvailable {
        get;
        set {
            field = value;
            IconNode.IconImage.MultiplyColor = value ? new Vector3(1.0f, 1.0f, 1.0f) : new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    /// <summary>
    /// Gets or sets whether the gear icon representing a macro is shown.
    /// </summary>
    public bool ShowMacroIcon {
        get;
        set {
            field = value;

            if (value) {
                IconNode.IconIndicator2.IconNode.PartId = 14;
            }
            IconNode.IconIndicator2.IconNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the primary resource cost text.
    /// </summary>
    public uint ResourceCost {
        get;
        set {
            field = value;
            IconNode.IconExtras.ResourceCostTextNode.String = value.ToString();
        }
    }

    /// <summary>
    /// Gets or sets whether the resource cost node should be visible.
    /// </summary>
    public bool ShowResourceCost {
        get => IconNode.IconExtras.ResourceCostTextNode.IsVisible;
        set => IconNode.IconExtras.ResourceCostTextNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the charge count.
    /// </summary>
    public uint ChargeCount {
        get;
        set {
            field = value;
            IconNode.IconExtras.ChargeCountImageNode.PartId = value;
        }
    }

    /// <summary>
    /// Gets or sets the charge cooldown percentage.
    /// </summary>
    public float ChargePercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.AlternateCooldownNode.CooldownImage.PartId = (uint) (value * 80 + 81);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the charge count node should be shown.
    /// </summary>
    public bool ShowChargeCount {
        get;
        set {
            field = value;
            IconNode.IconExtras.AlternateCooldownNode.IsVisible = value;
            IconNode.IconExtras.ChargeCountImageNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the cooldown percent background.
    /// </summary>
    public uint CooldownSeconds {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownTextNode.String = value.ToString();
        }
    }

    /// <summary>
    /// Gets or sets whether the cooldown text should be shown.
    /// </summary>
    public bool ShowCooldownSeconds {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownTextNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the cooldown percentage.
    /// </summary>
    public float CooldownPercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownNode.CooldownImage.PartId = (uint) (value * 80);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the cooldown progress should display.
    /// </summary>
    public bool ShowCooldownPercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownNode.GlossyImageFrame.IsVisible = !value;
            IconNode.IconExtras.CooldownNode.CooldownImage.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the color used for the cost text.
    /// </summary>
    public CostTextColor CostTextColor {
        get => IconNode.IconExtras.CostTextColor;
        set => IconNode.IconExtras.CostTextColor = value;
    }

    /// <summary>
    /// Gets or sets the keybind that will activate this slot.
    /// </summary>
    public KeySetting? KeyBind { get; set; }

    /// <summary>
    /// Gets this hotbar slot to the specific type and id.
    /// </summary>
    public void SetSlot(DragDropType type, uint id) {
        Payload.Type = type;
        Payload.Int2 = (int) id;

        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(Payload.Type), (uint) Payload.Int2);
    }

    /// <summary>
    /// Function that is called when the associated <see cref="KeyBind"/> is pressed.
    /// </summary>
    protected virtual void OnKeybindPressed()
        => OnHotbarNodeClicked(this);

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

        if (hotbarData.IsEmpty) return;

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
        }

        ShowTooltip();
    }

    private void OnHotbarNodeRollOut(DragDropNode thisNode) {
        HideTooltip();
    }

    private void OnHotbarNodePayloadAccepted(DragDropNode thisNode, DragDropPayload payload) {
        Payload.Type = payload.Type;
        Payload.Int2 = payload.Int2;

        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(payload.Type), (uint) payload.Int2);

        // Discard the source nod eif it's known.
        if (dragSourceNode is not null) {
            dragSourceNode.OnDiscard?.Invoke(dragSourceNode);
            dragSourceNode.Update();
        }
    }

    private unsafe void OnHotbarNodeClicked(DragDropNode thisNode) {
        var hotbarModule = RaptureHotbarModule.Instance();
        if (hotbarModule is null) return;

        fixed (RaptureHotbarModule.HotbarSlot* hotbarSlotData = &hotbarData) {
            hotbarModule->ExecuteSlot(hotbarSlotData);
        }
    }

    private void OnHotbarNodeDiscard(DragDropNode thisNode) {
        Payload.Clear();
        hotbarState = new Experimental.HotbarUiIntermediate();
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

        return $"{modifierKey}{(int)keyBind.Key - '0'}";
    }

    private RaptureHotbarModule.HotbarSlot hotbarData;
    private Experimental.HotbarUiIntermediate hotbarState;

    private static HotbarNode? dragSourceNode;
}
