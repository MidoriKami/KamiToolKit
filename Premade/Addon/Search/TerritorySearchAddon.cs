using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Utility;
using KamiToolKit.Dalamud;
using KamiToolKit.Enums;
using KamiToolKit.Premade.Node.ListItem;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.Addon.Search;

public class TerritorySearchAddon : BaseSearchAddon<TerritoryType, TerritoryTypeListItemNode> {
    public TerritorySearchAddon() {
        SearchOptions = Services.DataManager.GetExcelSheet<TerritoryType>()
            .Where(territory => territory.RowId is not 0)
            .Where(territory => territory.LoadingImage.RowId is not 0)
            .Where(territory => !territory.PlaceName.ValueNullable?.Name.ToString().IsNullOrEmpty() ?? false)
            .ToList();
    }

    protected override int Comparer(TerritoryType left, TerritoryType right, Enum sortingMode, bool reversed) {
        var result = sortingMode switch {
            DefaultSortOptions.Alphabetical => string.CompareOrdinal(left.Name.ToString(), right.Name.ToString()),
            DefaultSortOptions.Id => left.RowId.CompareTo(right.RowId),
            _ => 0,
        };

        return reversed ? -result : result;
    }

    protected override bool IsMatch(TerritoryType item, string searchString) {
        var regex = new Regex(searchString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        if (regex.IsMatch(item.RowId.ToString())) return true;
        if (regex.IsMatch(item.PlaceName.ValueNullable?.Name.ToString() ?? string.Empty)) return true;
        if (regex.IsMatch(item.ContentFinderCondition.ValueNullable?.Name.ToString() ?? string.Empty)) return true;

        return false;
    }
}
