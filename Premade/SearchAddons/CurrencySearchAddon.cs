using System.Collections.Generic;
using System.Linq;
using KamiToolKit.Classes;
using KamiToolKit.Premade.ListItemNodes;
using Lumina.Excel.Sheets;

namespace KamiToolKit.Premade.SearchAddons;

public class CurrencySearchAddon : ItemSearchAddonBase<CurrencyListItemNode> {
    public CurrencySearchAddon()
        => SearchOptions = GetCurrencyItems().ToList();

    private static IEnumerable<Item> GetCurrencyItems() {
        var dataManager = DalamudInterface.Instance.DataManager;

        var obsoleteTomes = dataManager.GetExcelSheet<TomestonesItem>()
            .Where(item => item.Tomestones.RowId is 0)
            .Select(item => item.Item.Value)
            .ToHashSet(EqualityComparer<Item>.Create(
            (x, y) => x.RowId == y.RowId,
            obj => obj.RowId.GetHashCode()
        ));

        return dataManager.GetExcelSheet<Item>()
            .Where(item => item is { Name.IsEmpty: false, ItemUICategory.RowId: 100 } or { RowId: >= 1 and < 100, Name.IsEmpty: false })
            .Where(item => !obsoleteTomes.Contains(item));
    }
}
