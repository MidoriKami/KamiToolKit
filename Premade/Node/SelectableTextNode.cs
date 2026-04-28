using System.Numerics;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Premade.Node;

public class SelectableTextNode : SelectableNode {
    public TextNode TextNode;

    public SelectableTextNode() {
        TextNode = new TextNode();
        TextNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        TextNode.Size = Size;
        TextNode.Position = new Vector2(6.0f, 0.0f);
    }

    public ReadOnlySeString String {
        get => TextNode.String;
        set => TextNode.String = value;
    }
}
