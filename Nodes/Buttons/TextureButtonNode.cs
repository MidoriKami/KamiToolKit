using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public class TextureButtonNode : ButtonBase {

    public readonly SimpleImageNode ImageNode;

    public TextureButtonNode() {
        ImageNode = new ImGuiImageNode {
            NodeId = 3, 
            WrapMode = WrapMode.Stretch, 
        };
        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    public Vector2 TextureCoordinates {
        get => ImageNode.TextureCoordinates;
        set => ImageNode.TextureCoordinates = value;
    }

    public Vector2 TextureSize {
        get => ImageNode.TextureSize;
        set => ImageNode.TextureSize = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ImageNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadTwoPartTimelines(this, ImageNode);
}
