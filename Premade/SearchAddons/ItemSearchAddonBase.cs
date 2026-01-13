using System.Text.RegularExpressions;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.SearchAddons;

public class ItemSearchAddonBase<T> : BaseSearchAddon<Item, T> where T : ListItemNode<Item>, new() {
    protected override int Comparer(Item left, Item right, string sortingString, bool reversed) {
        var result = sortingString switch {
            "Alphabetical" => string.CompareOrdinal(left.Name.ToString(), right.Name.ToString()),
            "Id" => left.RowId.CompareTo(right.RowId), 
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
