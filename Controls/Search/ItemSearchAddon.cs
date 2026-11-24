using System.Linq;
using KamiToolKit.Data.ListItemNodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// Specialization of <see cref="AbstractSearchAddon{T,TU}"/> that implements the results
/// with icons and item information.
/// </summary>
public class ItemSearchAddon : AbstractSearchAddon<Item, ItemListItemNode> {

    /// <inheritdoc />
    protected override void OnSearchInputReceived(ReadOnlySeString searchString) {
        var searchRegex = searchString.AsRegex();

        ResultsListNode?.Options = OptionsList.Where(option => {
            if (searchRegex.IsMatch(option.Name.ToString())) return true;

            var uiCategory = option.ItemUICategory.ValueNullable;
            if (uiCategory is not null && searchRegex.IsMatch(uiCategory.Value.Name.ToString())) return true;

            return false;
        }).ToList();

        base.OnSearchInputReceived(searchString);
    }
}
