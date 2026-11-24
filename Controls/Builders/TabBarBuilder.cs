using System;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Controls.Builders;

/// <summary>
/// Builder to build the initial entries of a tab bar.
/// </summary>
public class TabBarBuilder(TabBarNode node) {

    /// <summary>
    /// Add a tab by name.
    /// </summary>
    public TabBarBuilder AddTab(ReadOnlySeString label, Action onClick, ReadOnlySeString? tooltip = null) {
        node.AddTab(new TabBarEntry {
            Label = label,
            OnClick = onClick,
            Tooltip = tooltip,
        });
        return this;
    }

    /// <summary>
    /// Add a tab by a textId.
    /// </summary>
    public TabBarBuilder AddTab(uint textId, NodeData.SheetType sheetType, Action onClick, ReadOnlySeString? tooltip = null) {
        node.AddTab(new TabBarEntry {
            TextId = textId,
            SheetType = sheetType,
            OnClick = onClick,
            Tooltip = tooltip,
        });
        return this;
    }

    /// <summary>
    /// Adds a tab by tab entry.
    /// </summary>
    public TabBarBuilder AddTab(TabBarEntry entry) {
        node.AddTab(entry);
        return this;
    }
}
