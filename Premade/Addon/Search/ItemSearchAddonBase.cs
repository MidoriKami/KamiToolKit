using System;
using System.Text.RegularExpressions;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.Addon.Search;

public class ItemSearchAddonBase<T> : BaseSearchAddon<Item, T> where T : ListItemNode<Item>, IListItemNode, new() {
    protected override int Comparer(Item left, Item right, Enum sortingMode, bool reversed) {
        var result = sortingMode switch {
            DefaultSortOptions.Alphabetical => string.CompareOrdinal(left.Name.ToString(), right.Name.ToString()),
            DefaultSortOptions.Id => left.RowId.CompareTo(right.RowId), 
            _ => 0,
        };
        
        return reversed ? -result : result;
    }

    protected override bool IsMatch(Item item, string searchString) {
        var isDescriptionSearch = searchString.StartsWith('$');

        if (isDescriptionSearch) {
            searchString = searchString[1..];
        }
        
        var regex = new Regex(searchString,RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        
        if (regex.IsMatch(item.RowId.ToString())) return true;
        if (regex.IsMatch(item.Name.ToString())) return true;
        if (regex.IsMatch(item.Description.ToString()) && isDescriptionSearch) return true;
        if (regex.IsMatch(item.LevelEquip.ToString())) return true;
        if (regex.IsMatch(item.LevelItem.RowId.ToString())) return true;
        if (regex.IsMatch(item.ClassJobCategory.Value.Name.ToString())) return true;
        if (regex.IsMatch(item.ItemUICategory.Value.Name.ToString())) return true;

        return false;
    }
}
