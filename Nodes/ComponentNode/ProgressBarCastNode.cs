using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of the <see cref="ProgressNode"/> that represents a player-spell cast.
/// </summary>
public unsafe class ProgressBarCastNode : ProgressNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundImageNode;

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode ProgressNode;

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BorderImageNode;

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

    /// <summary>
    /// Gets or sets the border texture color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public Vector4 BorderColor {
        get => new(BorderImageNode.AddColor.X, BorderImageNode.AddColor.Y, BorderImageNode.AddColor.Z, BorderImageNode.ResNode->Color.A / 255.0f);
        set {
            BorderImageNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            BorderImageNode.AddColor = value.AsVector3Color();
        }
    }

    /// <summary>
    /// Gets or sets whether the border should be visible.
    /// </summary>
    public bool BorderVisible {
        get => BorderImageNode.IsVisible;
        set => BorderImageNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the multiply color for all node parts.
    /// </summary>
    /// <remarks>
    /// This can be used to darken everything to represent a "disabled" state.
    /// </remarks>
    public override Vector3 MultiplyColor {
        get => base.MultiplyColor;
        set {
            base.MultiplyColor = value;
            BackgroundImageNode.MultiplyColor = value;
            ProgressNode.MultiplyColor = value;
            BorderImageNode.MultiplyColor = value;
        }
    }

    public ProgressBarCastNode() {
        BackgroundImageNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/Parameter_Gauge.tex",
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 100.0f),
            LeftOffset = 20,
            RightOffset = 20,
        };
        BackgroundImageNode.AttachNode(this);

        ProgressNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/Parameter_Gauge.tex",
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 40.0f),
            MultiplyColor = new Vector3(90.0f, 75.0f, 75.0f) / 255.0f,
            AddColor = KnownColor.Yellow.Vector().AsVector3Color() / 255.0f,
            LeftOffset = 10,
            RightOffset = 10,
        };
        ProgressNode.AttachNode(this);

        BorderImageNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/Parameter_Gauge.tex",
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            LeftOffset = 20,
            RightOffset = 20,
        };
        BorderImageNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundImageNode.Size = Size;
        ProgressNode.Size = Size;
        BorderImageNode.Size = Size;
    }
}
