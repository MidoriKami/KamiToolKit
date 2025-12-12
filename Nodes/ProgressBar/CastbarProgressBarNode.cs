using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class CastBarProgressBarNode : SimpleComponentNode {

    [JsonProperty] public readonly NineGridNode BackgroundImageNode;
    [JsonProperty] public readonly NineGridNode ProgressNode;
    [JsonProperty] public readonly NineGridNode BorderImageNode;

    public CastBarProgressBarNode() {
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

    public float Progress {
        get => ProgressNode.Width / Width;
        set => ProgressNode.Width = Width * value;
    }

    public Vector4 BackgroundColor {
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

    public Vector4 BarColor {
        get => new(ProgressNode.AddColor.X, ProgressNode.AddColor.Y, ProgressNode.AddColor.Z, ProgressNode.ResNode->Color.A / 255.0f);
        set {
            ProgressNode.ResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            ProgressNode.AddColor = value.AsVector3Color();
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

    public override void DrawConfig() {
        base.DrawConfig();

        using (var background = ImRaii.TreeNode("Background")) {
            if (background) {
                BackgroundImageNode.DrawConfig();
            }
        }

        using (var progress = ImRaii.TreeNode("Progress")) {
            if (progress) {
                ProgressNode.DrawConfig();
            }
        }

        using (var border = ImRaii.TreeNode("Border")) {
            if (border) {
                BorderImageNode.DrawConfig();
            }
        }
    }
}
