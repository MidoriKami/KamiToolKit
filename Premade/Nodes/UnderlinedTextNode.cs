using System.Numerics;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Premade.Nodes;

public class UnderlinedTextNode : SimpleComponentNode {

    public readonly CategoryTextNode LabelTextNode;
    public readonly HorizontalLineNode LineNode;

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

    public ReadOnlySeString String {
        get => LabelTextNode.String;
        set {
            LabelTextNode.String = value;
            RecalculateLineSize();
        }
    }

    private void RecalculateLineSize() {
        var textSize = LabelTextNode.GetTextDrawSize();
        LineNode.Width = textSize.X + 32.0f;
    }
}
