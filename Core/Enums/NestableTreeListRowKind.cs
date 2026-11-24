using KamiToolKit.UI.Nodes.Layout;

namespace KamiToolKit.Core.Enums;

/// <summary>
/// Row kinds produced while laying out a <see cref="NestableTreeListNode{T,TU}"/>.
/// </summary>
public enum NestableTreeListRowKind {

    /// <summary>
    /// A collapsible group.
    /// </summary>
    Group,

    /// <summary>
    /// A regular leaf entry under a group.
    /// </summary>
    Entry,
}
