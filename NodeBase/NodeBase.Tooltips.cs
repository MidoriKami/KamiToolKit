using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

public unsafe partial class NodeBase {
    public virtual ReadOnlySeString? Tooltip {
        get;
        set {
            field = value;
            switch (value) {
                case { IsEmpty: false } when !TooltipRegistered:
                    AddEvent(AtkEventType.MouseOver, ShowTooltip);
                    AddEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled += ToggleCollisionFlag;

                    if (this is not ComponentNode && IsVisible) {
                        AddFlags(NodeFlags.HasCollision);
                    }

                    TooltipRegistered = true;
                    break;

                case null when TooltipRegistered: {
                    RemoveEvent(AtkEventType.MouseOver, ShowTooltip);
                    RemoveEvent(AtkEventType.MouseOut, HideTooltip);
                    OnVisibilityToggled -= ToggleCollisionFlag;

                    TooltipRegistered = false;
                    break;
                }
            }
        }
    }

    public string TooltipString {
        get => Tooltip?.ToString() ?? string.Empty;
        set => Tooltip = value;
    }

    protected bool TooltipRegistered { get; set; }

    public void ShowActionTooltip(uint actionId, string? textLabel = null)
        => AtkStage.Instance()->ShowActionTooltip(ResNode, actionId, textLabel);

    public void ShowItemTooltip(uint itemId)
        => AtkStage.Instance()->ShowItemTooltip(ResNode, itemId);

    public void ShowInventoryItemTooltip(InventoryType container, short slot)
        => AtkStage.Instance()->ShowInventoryItemTooltip(ResNode, container, slot);

    public void ShowTooltip() {
        if (Tooltip is {} tooltip && TooltipRegistered && ParentAddon is not null) {
            using var stringBuilder = new RentedSeStringBuilder();
            AtkStage.Instance()->TooltipManager.ShowTooltip(ParentAddon->Id, ResNode, stringBuilder.Builder.Append(tooltip).GetViewAsSpan());
        }
    }

    public void HideTooltip() {
        if (ParentAddon is null) return;

        AtkStage.Instance()->TooltipManager.HideTooltip(ParentAddon->Id);
    }
}
