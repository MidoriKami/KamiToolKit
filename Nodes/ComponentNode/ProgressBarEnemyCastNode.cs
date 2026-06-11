using System.Numerics;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="ProgressNode"/> to represent the castbar that enemies use.
/// </summary>
public unsafe class ProgressBarEnemyCastNode : ProgressNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundImageNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode ProgressNode { get; }

    /// <inheritdoc/>
    public override float Progress {
        get => ProgressNode.Width / Width;
        set => ProgressNode.Width = Width * value;
    }

    /// <inheritdoc/>
    public override Vector4 BackgroundColor {
        get => new(BackgroundImageNode.AddColor.X, BackgroundImageNode.AddColor.Y, BackgroundImageNode.AddColor.Z, BackgroundImageNode.ResNode->Color.A / 255.0f);
        set {
            BackgroundImageNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            BackgroundImageNode.AddColor = value.AsVector3Color();
        }
    }

    /// <inheritdoc/>
    public override Vector4 BarColor {
        get => new(ProgressNode.AddColor.X, ProgressNode.AddColor.Y, ProgressNode.AddColor.Z, ProgressNode.ResNode->Color.A / 255.0f);
        set {
            ProgressNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            ProgressNode.AddColor = value.AsVector3Color();
        }
    }

    public ProgressBarEnemyCastNode() {
        BackgroundImageNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/PartyList_GaugeCast.tex",
            TextureSize = new Vector2(204.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 12.0f),
            LeftOffset = 20,
            RightOffset = 20,
        };
        BackgroundImageNode.AttachNode(this);

        ProgressNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/PartyList_GaugeCast.tex",
            TextureSize = new Vector2(188.0f, 7.0f),
            TextureCoordinates = new Vector2(8.0f, 3.0f),
            LeftOffset = 10,
            RightOffset = 10,
        };
        ProgressNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundImageNode.Size = Size;
        ProgressNode.Size = Size;
    }
}
