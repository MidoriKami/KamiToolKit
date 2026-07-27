namespace KamiToolKit.Enums;

/// <summary>
/// Row kinds produced while laying out a <see cref="Nodes.NestableTreeListNode{T,TU}"/>.
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
