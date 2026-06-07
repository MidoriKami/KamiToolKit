using System;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes;

/// <summary>
/// Data entry representing a tab bar entry.
/// </summary>
public class TabBarEntry {

    /// <summary>
    /// The displayed label.
    /// </summary>
    public ReadOnlySeString Label { get; set; }

    /// <summary>
    /// Tooltip shown when hovering the tab.
    /// </summary>
    public ReadOnlySeString? Tooltip { get; set; }

    /// <summary>
    /// Game text id for loading a label automatically.
    /// </summary>
    public uint TextId { get; set; }

    /// <summary>
    /// Game text sheet type for loading a label automatically.
    /// </summary>
    public NodeData.SheetType SheetType { get; set; }

    /// <summary>
    /// The callback when this tab is clicked.
    /// </summary>
    public required Action OnClick { get; set; }
}
