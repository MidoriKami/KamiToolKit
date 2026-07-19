using System.Collections.Generic;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A <see cref="TreeListNode{T,TU}"/> that supports nested collapsible sections via <see cref="Sections"/>.
/// </summary>
/// <typeparam name="T">The data model to use.</typeparam>
/// <typeparam name="TU">The view to render the data models data.</typeparam>
public class NestableTreeListNode<T, TU> : TreeListNode<T, TU> where TU : TreeListItemNode<T>, ITreeListItemNode, new() {

    /// <summary>
    /// Gets or sets the nested section tree used to populate this list.
    /// </summary>
    public List<TreeListSection<T>> Sections {
        get;
        set {
            field = value;
            OnDataChanged(value.Count is 0);
        }
    } = [];

    /// <inheritdoc />
    protected override bool IsEmpty => Sections.Count is 0;

    /// <inheritdoc />
    protected override IEnumerable<VisibleRow> EnumerateVisibleRows() {
        foreach (var section in Sections) {
            foreach (var row in EnumerateSectionRows(section, parentPath: default)) {
                yield return row;
            }
        }
    }

    private IEnumerable<VisibleRow> EnumerateSectionRows(TreeListSection<T> section, ReadOnlySeString parentPath) {
        var path = parentPath.IsEmpty ? section.Header : $"{parentPath}/{section.Header}";
        yield return VisibleRow.ForHeader(section.Header, path);

        if (CollapsedEntries.Contains(path)) {
            yield break;
        }

        foreach (var entry in section.Entries) {
            yield return VisibleRow.ForEntry(entry);
        }

        foreach (var child in section.Children) {
            foreach (var row in EnumerateSectionRows(child, path)) {
                yield return row;
            }
        }
    }
}
