using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;

namespace KamiToolKit.Nodes;

public class ImGuiIconButtonNode : ButtonBase {

    public readonly NineGridNode BackgroundNode;
    public readonly ImGuiImageNode ImageNode;

    public ImGuiIconButtonNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/BgParts.tex",
            TextureSize = new Vector2(32.0f, 32.0f),
            TextureCoordinates = new Vector2(33.0f, 65.0f),
            TopOffset = 8.0f,
            LeftOffset = 8.0f,
            RightOffset = 8.0f,
            BottomOffset = 8.0f,
            NodeId = 2,
            IsVisible = true,
        };
        BackgroundNode.AttachNode(this);

        ImageNode = new ImGuiImageNode {
            IsVisible = true, NodeId = 3,
        };

        ImageNode.AttachNode(this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    public bool ShowBackground {
        get => BackgroundNode.IsVisible;
        set => BackgroundNode.IsVisible = value;
    }

    public string TexturePath {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ImageNode.Size = Size - new Vector2(16.0f, 16.0f);
        ImageNode.Position = BackgroundNode.Position + new Vector2(BackgroundNode.LeftOffset, BackgroundNode.TopOffset);
        BackgroundNode.Size = Size;
    }

    public void LoadTexture(IDalamudTextureWrap texture)
        => ImageNode.LoadTexture(texture);

    public void LoadTextureFromFile(string path)
        => ImageNode.LoadTextureFromFile(path);

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, ImageNode, new Vector2(8.0f, 8.0f));
}
