using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Classes;

/// <summary>
/// A single visible group or entry row for <see cref="NestableTreeListNode{T,TU}"/>.
/// </summary>
/// <typeparam name="T">The entry data model.</typeparam>
public readonly struct NestableTreeListRow<T> {

    /// <summary>
    /// Gets the kind of row this represents.
    /// </summary>
    public required NestableTreeListRowKind Kind { get; init; }

    /// <summary>
    /// Gets the group label when <see cref="Kind"/> is <see cref="NestableTreeListRowKind.Group"/>.
    /// </summary>
    public ReadOnlySeString Header { get; init; }

    /// <summary>
    /// Gets the collapse key for this group.
    /// </summary>
    public ReadOnlySeString Path { get; init; }

    /// <summary>
    /// Gets the entry data when <see cref="Kind"/> is <see cref="NestableTreeListRowKind.Entry"/>.
    /// </summary>
    public T? Entry { get; init; }

    /// <summary>
    /// Creates a group row.
    /// </summary>
    public static NestableTreeListRow<T> ForGroup(ReadOnlySeString header, ReadOnlySeString path) => new() {
        Kind = NestableTreeListRowKind.Group,
        Header = header,
        Path = path,
    };

    /// <summary>
    /// Creates an entry row.
    /// </summary>
    public static NestableTreeListRow<T> ForEntry(T entry) => new() {
        Kind = NestableTreeListRowKind.Entry,
        Entry = entry,
    };
}
