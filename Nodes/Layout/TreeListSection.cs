using System.Collections.Generic;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A collapsible section for <see cref="TreeListNode{T,TU}"/>, optionally containing nested child sections.
/// </summary>
/// <typeparam name="T">The entry data model.</typeparam>
public class TreeListSection<T> {

    /// <summary>
    /// Gets or sets the collapsing header label.
    /// </summary>
    public required ReadOnlySeString Header { get; set; }

    /// <summary>
    /// Gets or sets the entries shown directly under this section's header.
    /// </summary>
    public List<T> Entries { get; set; } = [];

    /// <summary>
    /// Gets or sets nested child sections shown after <see cref="Entries"/> when this section is expanded.
    /// </summary>
    public List<TreeListSection<T>> Children { get; set; } = [];
}
