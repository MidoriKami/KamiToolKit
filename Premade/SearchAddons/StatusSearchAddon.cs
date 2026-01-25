using System.Linq;
using System.Text.RegularExpressions;
using KamiToolKit.Classes;
using KamiToolKit.Premade.ListItemNodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.SearchAddons;

public class StatusSearchAddon : BaseSearchAddon<Status, StatusListItemNode> {
    public StatusSearchAddon() {
        SearchOptions = DalamudInterface.Instance.DataManager.GetExcelSheet<Status>()
            .Where(territory => territory.RowId is not 0)
            .Where(territory => !territory.Name.IsEmpty)
            .ToList();
    }
    
    protected override int Comparer(Status left, Status right, string sortingString, bool reversed){
        var result = sortingString switch {
            "Alphabetical" => string.CompareOrdinal(left.Name.ToString(), right.Name.ToString()),
            "Id" => left.RowId.CompareTo(right.RowId), 
            _ => 0,
        };
        
        return reversed ? -result : result;
    }

    protected override bool IsMatch(Status item, string searchString) {
        var regex = new Regex(searchString,RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        if (regex.IsMatch(item.RowId.ToString())) return true;
        if (regex.IsMatch(item.Name.ToString())) return true;

        return false;
    }
}
