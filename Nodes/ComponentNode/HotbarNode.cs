
using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="DragDropNode"/> that has handy accessors for things used to represent a hotbar slot.
/// </summary>
public class HotbarNode : DragDropNode {

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
        }
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
    /// Gets this hotbar slot to the specific type and id.
    /// </summary>
    public void SetSlot(DragDropType type, uint id) {
        Payload.Type = type;
        Payload.Int2 = (int) id;

        hotbarData.Set(UIGlobals.GetHotbarSlotTypeFromDragDropType(Payload.Type), (uint) Payload.Int2);
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
        OnRollOver = OnHotbarNodeRollOver;
        OnRollOut = OnHotbarNodeRollOut;
        OnPayloadAccepted = OnHotbarNodePayloadAccepted;
        OnClicked = OnHotbarNodeClicked;
        OnDiscard = OnHotbarNodeDiscard;
    }

    private void OnHotbarNodeRollOver(DragDropNode thisNode) {
        HideTooltip();

        switch (hotbarData.ApparentSlotType) {
            case RaptureHotbarModule.HotbarSlotType.Action:
                ActionTooltip = hotbarData.ApparentActionId;

                // Slot looks like an action, but is actually a macro, show macro name too!
                if (hotbarData.CommandType is RaptureHotbarModule.HotbarSlotType.Macro) {
                    TextTooltip = hotbarData.PopUpHelp.AsReadOnlySeString();
                }
                break;

            case RaptureHotbarModule.HotbarSlotType.Macro:
                TextTooltip = hotbarData.PopUpHelp.AsReadOnlySeString();
                ActionTooltip = 0;
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

    private RaptureHotbarModule.HotbarSlot hotbarData;
    private Experimental.HotbarUiIntermediate hotbarState;
}
