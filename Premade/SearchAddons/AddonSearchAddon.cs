using System.Collections.Generic;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Premade.ListItemNodes;

namespace KamiToolKit.Premade.SearchAddons;

public unsafe class AddonSearchAddon : BaseSearchAddon<Pointer<AtkUnitBase>, AddonListItemNode> {

    public AddonSearchAddon() {
        SearchOptions = GetAllAddons();
        SortingOptions = [ "Visibility", "Alphabetical" ];
        ItemSpacing = 3.0f;
    }

    protected override int Comparer(Pointer<AtkUnitBase> left, Pointer<AtkUnitBase> right, string sortingString, bool reversed) {
        if (left.Value is null || right.Value is null) return 0;

        switch (sortingString) {
            case "Alphabetical":
                return string.CompareOrdinal(left.Value->NameString, right.Value->NameString) * (reversed ? -1 : 1);

            case "Visibility":
                var visibilityComparison = right.Value->IsVisible.CompareTo(left.Value->IsVisible);
                if (visibilityComparison is 0) {
                    visibilityComparison = string.CompareOrdinal(left.Value->NameString, right.Value->NameString);
                }

                return visibilityComparison * (reversed ? -1 : 1);
        }

        return 0;
    }

    protected override bool IsMatch(Pointer<AtkUnitBase> item, string searchString) {
        var regex = new Regex(searchString,RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        
        return regex.IsMatch(item.Value->NameString);
    }

    private static List<Pointer<AtkUnitBase>> GetAllAddons() {
        List<Pointer<AtkUnitBase>> addons = [];
        
        foreach (var entry in RaptureAtkUnitManager.Instance()->AllLoadedUnitsList.Entries) {
            if (entry.Value is null) continue;
            addons.Add(entry);
        }
        
        return addons;
    }
}
