using System.Numerics;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A <see cref="TextNode"/> with a <see cref="HorizontalLineNode"/> underneath it for stylish texting.
/// </summary>
public class UnderlinedTextNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public CategoryTextNode LabelTextNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public HorizontalLineNode LineNode { get; }

    /// <summary>
    /// Gets or sets the displayed string.
    /// </summary>
    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set {
            LabelTextNode.String = value;
            RecalculateLineSize();
        }
    }

    public UnderlinedTextNode() {
        LabelTextNode = new CategoryTextNode();
        LabelTextNode.AttachNode(this);

        LineNode = new HorizontalLineNode {
            Height = 4.0f,
        };
        LineNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelTextNode.Size = new Vector2(Width, Height - 4.0f);
        LabelTextNode.Position = new Vector2(0.0f, 0.0f);

        LineNode.Position = new Vector2(0.0f, LabelTextNode.Bounds.Bottom - 4.0f);
        RecalculateLineSize();
    }

    private void RecalculateLineSize() {
        var textSize = LabelTextNode.GetTextDrawSize();
        LineNode.Width = textSize.X + 32.0f;
    }
}
