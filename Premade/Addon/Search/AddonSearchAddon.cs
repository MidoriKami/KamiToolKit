using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.ListItem;

namespace KamiToolKit.Premade.Addon.Search;

public unsafe class AddonSearchAddon : BaseSearchAddon<Pointer<AtkUnitBase>, AddonListItemNode> {
    public bool AllowNameplateResult { get; init; } = false;

    public AddonSearchAddon() {
        SearchOptions = GetAllAddons();
        SortingOptions = [AddonSearchSortOptions.Visibility, AddonSearchSortOptions.Alphabetical];
        ItemSpacing = 3.0f;
    }

    protected override int Comparer(Pointer<AtkUnitBase> left, Pointer<AtkUnitBase> right, Enum sortingMode, bool reversed) {
        if (left.Value is null || right.Value is null) return 0;

        switch (sortingMode) {
            case AddonSearchSortOptions.Alphabetical:
                return string.CompareOrdinal(left.Value->NameString, right.Value->NameString) * (reversed ? -1 : 1);

            case AddonSearchSortOptions.Visibility:
                var visibilityComparison = right.Value->IsVisible.CompareTo(left.Value->IsVisible);
                if (visibilityComparison is 0) {
                    visibilityComparison = string.CompareOrdinal(left.Value->NameString, right.Value->NameString);
                }

                return visibilityComparison * (reversed ? -1 : 1);
        }

        return 0;
    }

    protected override bool IsMatch(Pointer<AtkUnitBase> item, string searchString) {
        var regex = new Regex(searchString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        return regex.IsMatch(item.Value->NameString);
    }

    private List<Pointer<AtkUnitBase>> GetAllAddons() {
        List<Pointer<AtkUnitBase>> addons = [];

        foreach (var entry in RaptureAtkUnitManager.Instance()->AllLoadedUnitsList.Entries) {
            if (entry.Value is null) continue;
            if (entry.Value->NameString is "NamePlate" && !AllowNameplateResult) continue;
            addons.Add(entry);
        }

        return addons;
    }
}
