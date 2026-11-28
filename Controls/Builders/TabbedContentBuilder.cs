using System;
using KamiToolKit.Core.Primitives;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Builders;

/// <summary>
/// Builder helper for tabbed content nodes.
/// </summary>
public class TabbedContentBuilder(TabbedContentNode node) {

    /// <summary>
    /// Adds a tab using a VerticalListNode backed builder for the contents.
    /// </summary>
    public TabbedContentBuilder AddTab(ReadOnlySeString name, Action<NodeBuilder> buildTabContents, ReadOnlySeString? tooltip = null, Action? onTabChanged = null) {
        var innerBuilder = new NodeBuilder<VerticalListNode>();
        buildTabContents(innerBuilder);

        node.AddTab(name, innerBuilder.Build(), tooltip, onTabChanged);
        return this;
    }

    /// <summary>
    /// Adds a tab using the provided node as the contents.
    /// </summary>
    public TabbedContentBuilder AddTab<T>(ReadOnlySeString name, T tabContents, ReadOnlySeString? tooltip = null, Action? onTabChanged = null) where T : NodeBase {
        node.AddTab(name, tabContents, tooltip, onTabChanged);

        return this;
    }
}
