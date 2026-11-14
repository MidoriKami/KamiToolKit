using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.BaseTypes;

public unsafe partial class NodeBase : ITextTooltip, IActionTooltip, IItemTooltip, IInventoryItemTooltip {

    /// <inheritdoc/>
    public virtual ReadOnlySeString TextTooltip {
        get;
        set {
            // If they are both empty, do nothing.
            if (field.IsEmpty && value.IsEmpty) return;

            // As long as both aren't empty, we can compare them.
            if (!field.IsEmpty && !value.IsEmpty) {
                if (field == value) return;
            }

            // else if one is empty and the other isn't then they are different,
            // and we need to set up tooltips.

            // If we are setting a valid value, try to enable text tooltips.
            if (!value.IsEmpty) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Text;
            }

            // If we are setting a empty value, disable text tooltips.
            else {
                tooltipType &= ~AtkTooltipType.Text;
            }

            if (ParentAddon is not null) {
                ParentAddon->UpdateCollisionNodeList(false);
            }

            field = value;
        }
    }

    /// <inheritdoc/>
    public void ShowTextTooltip()
        => ShowTooltip();

    /// <inheritdoc/>
    public void HideTextTooltip()
        => HideTooltip();


    /// <inheritdoc/>
    public virtual uint ActionTooltip {
        get;
        set {
            if (field == value) return;

            if (value is not 0) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Action;
            }
            else {
                tooltipType &= ~AtkTooltipType.Action;
            }

            if (ParentAddon is not null) {
                ParentAddon->UpdateCollisionNodeList(false);
            }

            field = value;
        }
    }

    /// <inheritdoc/>
    public void ShowActionTooltip()
        => ShowTooltip();

    /// <inheritdoc/>
    public void HideActionTooltip()
        => HideTooltip();

    /// <inheritdoc/>
    public virtual uint ItemTooltip {
        get;
        set {
            if (field == value) return;

            if (value is not 0) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Item;
            }
            else {
                tooltipType &= ~AtkTooltipType.Item;
            }

            if (ParentAddon is not null) {
                ParentAddon->UpdateCollisionNodeList(false);
            }

            field = value;
        }
    }

    /// <inheritdoc/>
    public void ShowItemTooltip()
        => ShowTooltip();

    /// <inheritdoc/>
    public void HideItemTooltip()
        => HideTooltip();

    /// <inheritdoc/>
    public virtual InventoryItemTooltip? InventoryItemTooltip {
        get;
        set {
            if (field == value) return;

            if (value is not null) {
                TryRegisterTooltipEvents();
                tooltipType |= AtkTooltipType.Item;
            }
            else {
                tooltipType &= ~AtkTooltipType.Item;
            }

            if (ParentAddon is not null) {
                ParentAddon->UpdateCollisionNodeList(false);
            }

            field = value;
        }
    }

    /// <inheritdoc/>
    public void ShowInventoryItemTooltip()
        => ShowTooltip();

    /// <inheritdoc/>
    public void HideInventoryItemTooltip()
        => HideTooltip();

    /// <summary>
    /// Property that indicates if a tooltip is already registered.
    /// </summary>
    /// <remarks>
    /// Used by inherited nodes if they want to override the tooltip behavior.
    /// </remarks>
    protected bool TooltipRegistered { get; set; }

    /// <summary>
    /// Triggers this node's tooltip to show, prioritizing Text -> Action -> Item tooltips in that order, only one tooltip will show.
    /// </summary>
    public virtual void ShowTooltip() {
        if (ParentAddon is null) return; // Shouldn't be possible
        if (tooltipType is AtkTooltipType.None) return;

        using var stringBuilder = new RentedSeStringBuilder();
        using var stringBuffer = new RentedAtkValues(1);
        if (!TextTooltip.IsEmpty) {
            stringBuffer[0].SetManagedString(stringBuilder.Append(TextTooltip).GetViewAsSpan());
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
        if (this is ComponentNode.ComponentNode) return;

        if (isVisible) {
            AddNodeFlags(NodeFlags.HasCollision);
        }
        else {
            RemoveNodeFlags(NodeFlags.HasCollision);
        }
    }

    private AtkTooltipType tooltipType = AtkTooltipType.None;
    private bool tooltipEventsRegistered;
}
