using System.Drawing;
using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Nodes;

public unsafe class ProgressBarCastNode : ProgressNode {

    public readonly NineGridNode BackgroundImageNode;
    public readonly NineGridNode ProgressNode;
    public readonly NineGridNode BorderImageNode;

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

    public override float Progress {
        get => ProgressNode.Width / Width;
        set => ProgressNode.Width = Width * value;
    }

    public override Vector4 BackgroundColor {
        get => new(BackgroundImageNode.AddColor.X, BackgroundImageNode.AddColor.Y, BackgroundImageNode.AddColor.Z, BackgroundImageNode.ResNode->Color.A / 255.0f);
        set {
            BackgroundImageNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            BackgroundImageNode.AddColor = value.AsVector3Color();
        }
    }

    public Vector4 BorderColor {
        get => new(BorderImageNode.AddColor.X, BorderImageNode.AddColor.Y, BorderImageNode.AddColor.Z, BorderImageNode.ResNode->Color.A / 255.0f);
        set {
            BorderImageNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            BorderImageNode.AddColor = value.AsVector3Color();
        }
    }

    public override Vector4 BarColor {
        get => new(ProgressNode.AddColor.X, ProgressNode.AddColor.Y, ProgressNode.AddColor.Z, ProgressNode.ResNode->Color.A / 255.0f);
        set {
            ProgressNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            ProgressNode.AddColor = value.AsVector3Color();
        }
    }

    public override Vector3 MultiplyColor {
        get => base.MultiplyColor;
        set {
            base.MultiplyColor = value;
            BackgroundImageNode.MultiplyColor = value;
            ProgressNode.MultiplyColor = value;
            BorderImageNode.MultiplyColor = value;
        }
    }

    public bool BorderVisible {
        get => BorderImageNode.IsVisible;
        set => BorderImageNode.IsVisible = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundImageNode.Size = Size;
        ProgressNode.Size = Size;
        BorderImageNode.Size = Size;
    }
}
