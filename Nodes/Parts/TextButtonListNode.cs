namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="ButtonListNode{T}"/>, for use in <see cref="TextDropDownNode"/>.
/// Not intended for external use.
/// </summary>
public class TextButtonListNode : ButtonListNode<string> {
    protected override string GetLabelForOption(string option) => option;
}
