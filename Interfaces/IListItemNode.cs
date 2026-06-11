using KamiToolKit.Nodes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface for <see cref="ListNode{T,TU}"/>
/// </summary>
public interface IListItemNode {

    /// <summary>
    /// Gets the height of this list item's entry.
    /// This is a statically per-type defined value.
    /// </summary>
    abstract static float ItemHeight { get; }
}
