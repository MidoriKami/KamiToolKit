using System.Numerics;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Further specialization of <see cref="SelectableNode"/> implementing a string display.
/// </summary>
public class SelectableTextNode : SelectableNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode TextNode { get; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => TextNode.String;
        set => TextNode.String = value;
    }

    public SelectableTextNode() {
        TextNode = new TextNode();
        TextNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        TextNode.Size = Size;
        TextNode.Position = new Vector2(6.0f, 0.0f);
    }
}
