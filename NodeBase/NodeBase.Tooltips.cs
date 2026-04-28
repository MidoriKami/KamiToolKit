using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

public record InventoryItemTooltip(InventoryType Inventory, short Slot);

public unsafe partial class NodeBase {

    private AtkTooltipType tooltipType = AtkTooltipType.None;
    private bool tooltipEventsRegistered;
    
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

    public void ShowTooltip() {
        if (ParentAddon is null) return; // Shouldn't be possible
        if (tooltipType is AtkTooltipType.None) return;

        using var stringBuilder = new RentedSeStringBuilder();
        using var stringBuffer = new AtkValue();
        if (!TextTooltip.IsEmpty) {
            stringBuffer.SetManagedString(stringBuilder.Builder.Append(TextTooltip).GetViewAsSpan());
        }
        
        var tooltipArgs = new AtkTooltipManager.AtkTooltipArgs();

        if (tooltipType.HasFlag(AtkTooltipType.Text)) {
            tooltipArgs.TextArgs.AtkArrayType = 0;
            tooltipArgs.TextArgs.Text = stringBuffer.String;
        }

        if (tooltipType.HasFlag(AtkTooltipType.Action)) {
            tooltipArgs.ActionArgs.Flags = 1;
            tooltipArgs.ActionArgs.Kind = DetailKind.Action;
            tooltipArgs.ActionArgs.Id = (int)ActionTooltip;
        }

        if (tooltipType.HasFlag(AtkTooltipType.Item) && InventoryItemTooltip is {} inventoryTooltip) {
            tooltipArgs.ItemArgs.Kind = DetailKind.InventoryItem;
            tooltipArgs.ItemArgs.InventoryType = inventoryTooltip.Inventory;
            tooltipArgs.ItemArgs.Slot = inventoryTooltip.Slot;
            tooltipArgs.ItemArgs.BuyQuantity = -1;
            tooltipArgs.ItemArgs.Flag1 = 0;
        }
        else if (tooltipType.HasFlag(AtkTooltipType.Item) && InventoryItemTooltip is null) {
            tooltipArgs.ItemArgs.Kind = DetailKind.Item;
            tooltipArgs.ItemArgs.ItemId = (int) ItemTooltip;
            tooltipArgs.ItemArgs.BuyQuantity = -1;
            tooltipArgs.ItemArgs.Flag1 = 0;
        }
        
        AtkStage.Instance()->TooltipManager.ShowTooltip(tooltipType, ParentAddon->Id, this, &tooltipArgs);
    }

    public void ShowTextTooltip(ReadOnlySeString tooltip) {
        if (tooltip.IsEmpty) return;

        AtkStage.Instance()->TooltipManager.ShowTooltip(ParentAddon->Id, null, tooltip);
    }

    public void HideTooltip() {
        if (ParentAddon is null) return;

        AtkStage.Instance()->TooltipManager.HideTooltip(ParentAddon->Id);
    }
}
