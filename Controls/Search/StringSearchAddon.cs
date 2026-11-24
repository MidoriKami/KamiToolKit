using System.Linq;
using KamiToolKit.Data.ListItemNodes;
using KamiToolKit.Core.Extensions;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// Specialization of <see cref="AbstractSearchAddon{T,TU}"/> that implements the results as a simple string.
/// </summary>
public class StringSearchAddon : AbstractSearchAddon<string, StringListItemNode> {

    /// <inheritdoc />
    protected override void OnSearchInputReceived(ReadOnlySeString searchString) {
        var searchRegex = searchString.AsRegex();

        ResultsListNode?.Options = OptionsList.Where(option => searchRegex.IsMatch(option)).ToList();

        base.OnSearchInputReceived(searchString);
    }
}
