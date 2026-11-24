using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Data.ListItemNodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// An implementation of <see cref="AbstractSearchAddon{T,TU}"/> for selecting a currently loaded <see cref="AtkUnitBase"/>
/// </summary>
public class WindowSearchAddon : AbstractSearchAddon<Pointer<AtkUnitBase>, WindowListItemNode> {

    /// <summary>
    /// Overrides OptionsList to reorder options according to addon visibility to put visible options at the top.
    /// </summary>
    public override unsafe List<Pointer<AtkUnitBase>> OptionsList {
        get => base.OptionsList;
        set {
            base.OptionsList = value
                .OrderByDescending(option => option.Value is not null && option.Value->IsVisible)
                .ToList();
        }
    }

    /// <inheritdoc/>
    protected override unsafe void OnSearchInputReceived(ReadOnlySeString searchString) {
        var searchRegex = searchString.AsRegex();

        ResultsListNode?.Options = OptionsList.Where(option => {
            if (option.Value is null) return false;
            if (searchRegex.IsMatch(option.Value->NameString)) return true;

            return false;
        }).ToList();

        base.OnSearchInputReceived(searchString);
    }
}
