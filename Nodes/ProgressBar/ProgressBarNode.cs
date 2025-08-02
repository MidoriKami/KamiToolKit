using System.Numerics;

namespace KamiToolKit.Nodes;

public class ProgressBarNode : SimpleComponentNode {

    public readonly NineGridNode BackgroundNode;
    public readonly NineGridNode ForegroundNode;

    public ProgressBarNode() {
        BackgroundNode = new SimpleNineGridNode {
            NodeId = 2,
            TexturePath = "ui/uld/ToDoList.tex",
            TextureCoordinates = new Vector2(108.0f, 8.0f),
            TextureSize = new Vector2(44.0f, 12.0f),
            IsVisible = true,
            LeftOffset = 6,
            RightOffset = 6,
        };

        BackgroundNode.AttachNode(this);

        ForegroundNode = new SimpleNineGridNode {
            NodeId = 3,
            TexturePath = "ui/uld/ToDoList.tex",
            TextureCoordinates = new Vector2(112.0f, 0.0f),
            TextureSize = new Vector2(40.0f, 8.0f),
            IsVisible = true,
            LeftOffset = 4,
            RightOffset = 4,
        };

        ForegroundNode.AttachNode(this);
    }

    public Vector4 BackgroundColor {
        get => BackgroundNode.Color;
        set => BackgroundNode.Color = value;
    }

    public Vector4 BarColor {
        get => ForegroundNode.Color;
        set => ForegroundNode.Color = value;
    }

    public float Progress {
        get => ForegroundNode.Width / Width;
        set => ForegroundNode.Width = Width * value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;
        ForegroundNode.Size = Size;
    }
}
