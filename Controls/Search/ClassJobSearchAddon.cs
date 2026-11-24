using System.Linq;
using KamiToolKit.Data.ListItemNodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// Implementation of <see cref="AbstractSearchAddon{T,TU}"/> to implement a ClassJob search.
/// </summary>
public class ClassJobSearchAddon : AbstractSearchAddon<ClassJob, ClassJobListItemNode> {

    /// <inheritdoc />
    protected override void OnSearchInputReceived(ReadOnlySeString searchString) {
        var searchRegex = searchString.AsRegex();

        ResultsListNode?.Options = OptionsList.Where(option => {
            if (searchRegex.IsMatch(option.Name.ToString())) return true;
            if (searchRegex.IsMatch(option.Abbreviation.ToString())) return true;

            return false;
        }).ToList();

        base.OnSearchInputReceived(searchString);
    }
}
