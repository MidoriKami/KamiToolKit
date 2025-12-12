using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Premade.Nodes;

public unsafe class AddonStringInfoNode : StringInfoNode {
    public override string GetSubLabel()
        => IsVisible() ? "Visible" : "Hidden";

    public override uint? GetId() {
        var addon = GetAddon();
        if (addon == null) return null;

        return addon->Id;
    }

    public override uint? GetIconId()
        => IsVisible() ? (uint) 60071 : 60072;

    public override string? GetTexturePath()
        => null;

    public override int Compare(IInfoNodeData other, string sortingMode) {
        switch (sortingMode) {
            case "Alphabetical":
                return string.CompareOrdinal(Label, (other as AddonStringInfoNode)?.Label);

            case "Visibility":
                var visibilityComparison = (other as AddonStringInfoNode)?.IsVisible().CompareTo(IsVisible()) ?? 0;
                if (visibilityComparison is 0) {
                    visibilityComparison = string.CompareOrdinal(Label, (other as AddonStringInfoNode)?.Label);
                }

                return visibilityComparison;

            default:
                return base.Compare(other, sortingMode);
        }
    }

    private bool IsVisible() {
        var addon = GetAddon();
        return addon != null && addon->IsVisible;
    }

    private AtkUnitBase* GetAddon() => RaptureAtkUnitManager.Instance()->GetAddonByName(Label);
}
