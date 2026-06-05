using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

/// <summary>
/// NodeBase partial responsible for managing this node's tooltip.
/// </summary>
public unsafe partial class NodeBase {

    /// <summary>
    /// Gets or sets the Text Tooltip for this node.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    public virtual ReadOnlySeString TextTooltip {
        get;
        set {
            field = value;
            if (!value.IsEmpty) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Text;
            }
            else {
                tooltipType &= ~AtkTooltipType.Text;
            }
        }
    }

    /// <summary>
    /// Gets or sets the Action Tooltip for this node. Uses ActionId.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    public virtual uint ActionTooltip {
        get;
        set {
            field = value;
            if (value is not 0) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Action;
            }
            else {
                tooltipType &= ~AtkTooltipType.Action;
            }
        }
    }

    /// <summary>
    /// Gets or sets the Action Tooltip for this node. Uses ItemId.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    public virtual uint ItemTooltip {
        get;
        set {
            field = value;
            if (value is not 0) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Item;
            }
            else {
                tooltipType &= ~AtkTooltipType.Item;
            }
        }
    }

    /// <summary>
    /// Gets or sets the Action Tooltip for this node. Takes a InventoryType and a slot index to represent the item in that slot.
    /// </summary>
    /// <remarks>
    /// If tooltip is set after the node is attached, a collision update will be required.
    /// </remarks>
    public virtual InventoryItemTooltip? InventoryItemTooltip {
        get;
        set {
            field = value;
            if (value is not null) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Item;
            }
            else {
                tooltipType &= ~AtkTooltipType.Item;
            }
        }
    }

    /// <summary>
    /// Triggers this nodes tooltip to show, prioritizing Text -> Action -> Item tooltips in that order, only one tooltip will show.
    /// </summary>
    public virtual void ShowTooltip() {
        if (ParentAddon is null) return; // Shouldn't be possible
        if (tooltipType is AtkTooltipType.None) return;

        using var stringBuilder = new RentedSeStringBuilder();
        using var stringBuffer = new RentedAtkValues(1);
        if (!TextTooltip.IsEmpty) {
            stringBuffer[0].SetManagedString(stringBuilder.Builder.Append(TextTooltip).GetViewAsSpan());
        }

        var tooltipArgs = new AtkTooltipManager.AtkTooltipArgs();

        if (tooltipType.HasFlag(AtkTooltipType.Text)) {
            tooltipArgs.TextArgs.AtkArrayType = 0;
            tooltipArgs.TextArgs.Text = stringBuffer[0].String;
        }

        if (tooltipType.HasFlag(AtkTooltipType.Action)) {
            tooltipArgs.ActionArgs.Flags = 1;
            tooltipArgs.ActionArgs.Kind = DetailKind.Action;
            tooltipArgs.ActionArgs.Id = (int)ActionTooltip;
        }

        if (tooltipType.HasFlag(AtkTooltipType.Item) && InventoryItemTooltip is { } inventoryTooltip) {
            tooltipArgs.ItemArgs.Kind = DetailKind.InventoryItem;
            tooltipArgs.ItemArgs.InventoryType = inventoryTooltip.Inventory;
            tooltipArgs.ItemArgs.Slot = inventoryTooltip.Slot;
            tooltipArgs.ItemArgs.BuyQuantity = -1;
            tooltipArgs.ItemArgs.Flag1 = 0;
        }
        else if (tooltipType.HasFlag(AtkTooltipType.Item) && InventoryItemTooltip is null) {
            tooltipArgs.ItemArgs.Kind = DetailKind.Item;
            tooltipArgs.ItemArgs.ItemId = (int)ItemTooltip;
            tooltipArgs.ItemArgs.BuyQuantity = -1;
            tooltipArgs.ItemArgs.Flag1 = 0;
        }

        AtkStage.Instance()->TooltipManager.ShowTooltip(tooltipType, ParentAddon->Id, this, &tooltipArgs);
    }

    /// <summary>
    /// Shows the specified text as a tooltip for this node.
    /// </summary>
    public void ShowTextTooltip(ReadOnlySeString tooltip) {
        if (tooltip.IsEmpty) return;

        AtkStage.Instance()->TooltipManager.ShowTooltip(ParentAddon->Id, null, tooltip);
    }

    /// <summary>
    /// Hides any tooltip active for the addon this node is attached to.
    /// </summary>
    /// <remarks>
    /// You could potentially close a tooltip the game itself is showing you via this method, exercise caution.
    /// </remarks>
    public void HideTooltip() {
        if (ParentAddon is null) return;

        AtkStage.Instance()->TooltipManager.HideTooltip(ParentAddon->Id);
    }

    private void TryRegisterTooltipEvents() {
        if (tooltipEventsRegistered) return;

        AddEvent(AtkEventType.MouseOver, ShowTooltip);
        AddEvent(AtkEventType.MouseOut, HideTooltip);
        OnVisibilityToggled += ToggleCollisionFlag;
        ToggleCollisionFlag(IsVisible);

        tooltipEventsRegistered = true;
    }

    private void UnregisterTooltipEvents() {
        if (tooltipEventsRegistered) {
            RemoveEvent(AtkEventType.MouseOver, ShowTooltip);
            RemoveEvent(AtkEventType.MouseOut, HideTooltip);
            OnVisibilityToggled -= ToggleCollisionFlag;
            tooltipEventsRegistered = false;
        }
    }

    private void ToggleCollisionFlag(bool isVisible) {
        if (this is ComponentNode) return;

        if (isVisible) {
            AddNodeFlags(NodeFlags.HasCollision);
        }
        else {
            RemoveNodeFlags(NodeFlags.HasCollision);
        }
    }

    protected bool TooltipRegistered { get; set; }

    private AtkTooltipType tooltipType = AtkTooltipType.None;
    private bool tooltipEventsRegistered;
}
