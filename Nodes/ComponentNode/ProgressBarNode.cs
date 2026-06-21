using System.Numerics;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="ProgressBarNode"/> representing a general purpose progress display.
/// Modeled off the levequest progress display.
/// </summary>
public class ProgressBarNode : ProgressNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode ForegroundNode { get; }

    /// <inheritdoc/>
    public override Vector4 BackgroundColor {
        get => BackgroundNode.Color;
        set => BackgroundNode.Color = value;
    }

    /// <inheritdoc/>
    public override Vector4 BarColor {
        get => ForegroundNode.Color;
        set => ForegroundNode.Color = value;
    }

    /// <inheritdoc/>
    public override float Progress {
        get => ForegroundNode.Width / Width;
        set => ForegroundNode.Width = Width * value;
    }


    /// <summary>
    /// Constructs a new <see cref="ProgressBarNode"/>.
    /// </summary>
    public ProgressBarNode() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ToDoList.tex",
            TextureCoordinates = new Vector2(108.0f, 8.0f),
            TextureSize = new Vector2(44.0f, 12.0f),
            LeftOffset = 6,
            RightOffset = 6,
        };
        BackgroundNode.AttachNode(this);

        ForegroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ToDoList.tex",
            TextureCoordinates = new Vector2(112.0f, 0.0f),
            TextureSize = new Vector2(40.0f, 8.0f),
            LeftOffset = 4,
            RightOffset = 4,
        };
        ForegroundNode.AttachNode(this);
    }


    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Size = Size;
        ForegroundNode.Size = Size;
    }
}
