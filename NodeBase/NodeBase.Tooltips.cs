using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

public unsafe partial class NodeBase {
    private Utf8String* textTooltipBuffer;

    private bool tooltipPendingRegistration;
    private bool tooltipRegistered;

    public virtual ReadOnlySeString TextTooltip {
        get;
        set {
            if (value.IsEmpty) return;
            field = value;

            SetTextTooltip(value);
        }
    }

    private void SetTextTooltip(ReadOnlySeString value) {
        UnregisterTooltip();
            
        if (textTooltipBuffer is not null) {
            textTooltipBuffer->Dtor();
            textTooltipBuffer = null;
        }

        textTooltipBuffer = Utf8String.FromString(value.ToString());

        if (parentNode is not null) {
            RegisterTooltip();
        }
        else {
            tooltipPendingRegistration = true;
        }
    }

    private void UpdateTooltipCollision(bool isVisible) {
        if (this is ComponentNode) return;

        if (isVisible) {
            AddFlags(NodeFlags.HasCollision, NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
        }
        else {
            RemoveFlags(NodeFlags.HasCollision, NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
        }
    }

    private void RegisterTooltip() {
        if (textTooltipBuffer is null) return;
        if (ParentAddon is null) return;
        
        var args = new AtkTooltipManager.AtkTooltipArgs {
            TextArgs = new AtkTooltipManager.AtkTooltipArgs.AtkTooltipTextArgs {
                AtkArrayType = 0,
                Text = textTooltipBuffer->StringPtr,
            },
        };
        
        AddFlags(NodeFlags.HasCollision, NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);

        AtkStage.Instance()->TooltipManager.AttachTooltip(AtkTooltipManager.AtkTooltipType.Text, ParentAddon->Id, this, &args);
        
        OnVisibilityToggled += UpdateTooltipCollision;
        tooltipPendingRegistration = false;
        tooltipRegistered = true;
    }
    
    private void UnregisterTooltip() {
        if (!tooltipRegistered) return;
        
        AtkStage.Instance()->TooltipManager.DetachTooltip(this);

        OnVisibilityToggled -= UpdateTooltipCollision;
        tooltipRegistered = false;
        tooltipPendingRegistration = true;
    }

    public void ShowActionTooltip(uint actionId, string? textLabel = null)
        => AtkStage.Instance()->ShowActionTooltip(ResNode, actionId, textLabel);

    public void ShowItemTooltip(uint itemId)
        => AtkStage.Instance()->ShowItemTooltip(ResNode, itemId);

    public void ShowInventoryItemTooltip(InventoryType container, short slot)
        => AtkStage.Instance()->ShowInventoryItemTooltip(ResNode, container, slot);
}
