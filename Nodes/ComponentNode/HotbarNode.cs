
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

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

        fixed (RaptureHotbarModule.HotbarSlot* data = &hotbarData)
        fixed (Experimental.HotbarUiIntermediate* state = &HotbarState)
        {
            RaptureHotbarModule.HotbarSlotType outType;
            uint outActionId;
            ushort unkC4;

            RaptureHotbarModule.GetSlotAppearance(&outType, &outActionId, &unkC4, hotbarModule, data);
            hotbarData.ApparentActionId = outActionId;
            hotbarData.ApparentSlotType = outType;

            Experimental.UpdateHotbarSlotIntermediateData?.Invoke(RaptureHotbarModule.Instance(), data, state);

            IconId = HotbarState.IconId;

            ShowResourceCost = HotbarState.CostType is 2;
            ResourceCost = HotbarState.CostValue;

            ShowChargeCount = HotbarState.CostType is 0;
            ChargeCount = HotbarState.CurrentCharges;
            ChargePercent = HotbarState.ChargePercent / 100.0f;

            ShowCooldownSeconds = HotbarState.CooldownSeconds is not 0;
            CooldownSeconds = HotbarState.CooldownSeconds;

            ShowCooldownPercent = HotbarState.CooldownPercent is not 0;
            CooldownPercent = HotbarState.CooldownPercent / 100.0f;
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
    /// Sets this hotbar slot to the specified action.
    /// </summary>
    /// <param name="actionId"></param>
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

    // todo: figure out how to make this behave better
    private void OnHotbarNodeRollOver(DragDropNode thisNode) {
        switch (Payload.Type) {
            case DragDropType.Action:
                HideTooltip();
                ActionTooltip = (uint) Payload.Int2;
                ShowTooltip();
                break;
        }
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
        // todo: this
    }

    private RaptureHotbarModule.HotbarSlot hotbarData;

    // todo: only available temporarily for debugging.
    /// <summary>
    /// Gets the current hotbars state values. Not intended for external use.
    /// </summary>
    public readonly Experimental.HotbarUiIntermediate HotbarState;
}
