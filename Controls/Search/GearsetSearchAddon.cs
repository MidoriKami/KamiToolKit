using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Data.ListItemNodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// Implementation of <see cref="AbstractSearchAddon{T,TU}"/> for use with Gearsets.
/// By default, will populate <see cref="AbstractSearchAddon{T,TU}.OptionsList"/> with all available gearsets on open.
/// </summary>
public class GearsetSearchAddon : AbstractSearchAddon<RaptureGearsetModule.GearsetEntry, GearsetListItemNode> {

    /// <inheritdoc />
    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) {
        OptionsList = RaptureGearsetModule.Instance()->Entries
            .ToArray()
            .Where(entry => entry.Flags.HasFlag(RaptureGearsetModule.GearsetFlag.Exists))
            .OrderBy(entry => entry.Id)
            .ToList();

        base.OnSetup(addon, atkValueSpan);
    }

    /// <inheritdoc />
    protected override void OnSearchInputReceived(ReadOnlySeString searchString) {
        var searchRegex = searchString.AsRegex();

        ResultsListNode?.Options = OptionsList.Where(option => searchRegex.IsMatch(option.NameString)).ToList();

        base.OnSearchInputReceived(searchString);
    }
}
