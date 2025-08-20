using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class CastBarProgressBarNode : SimpleComponentNode {

    [JsonProperty] public readonly SimpleNineGridNode BackgroundImageNode;
    [JsonProperty] public readonly SimpleNineGridNode BorderImageNode;
    [JsonProperty] public readonly SimpleNineGridNode ProgressNode;

    // Recommended aspect ratio for width:height is 8:1
    public CastBarProgressBarNode() {
        BackgroundImageNode = new SimpleNineGridNode {
            NodeId = 2,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 100.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = 4,
            LeftOffset = 15,
            RightOffset = 15,
            TexturePath = "ui/uld/Parameter_Gauge.tex",
        };

        BackgroundImageNode.AttachNode(this);

        BorderImageNode = new SimpleNineGridNode {
            NodeId = 4,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = 4,
            LeftOffset = 15,
            RightOffset = 15,
            TexturePath = "ui/uld/Parameter_Gauge.tex",
        };

        BorderImageNode.AttachNode(this);

        ProgressNode = new SimpleNineGridNode {
            NodeId = 3,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 40.0f),
            NodeFlags = NodeFlags.Visible,
            Width = 80,
            Color = KnownColor.Yellow.Vector(),
            LeftOffset = 10,
            RightOffset = 10,
            PartsRenderType = 4,
            TexturePath = "ui/uld/Parameter_Gauge.tex",
        };

        ProgressNode.AttachNode(this);
    }

    public float Progress {
        get => ProgressNode.Width / Width;
        set => ProgressNode.Width = Width * value;
    }

    public Vector4 BackgroundColor {
        get => new(BackgroundImageNode.AddColor.X, BackgroundImageNode.AddColor.Y, BackgroundImageNode.AddColor.Z, BackgroundImageNode.InternalResNode->Color.A / 255.0f);
        set {
            BackgroundImageNode.InternalResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            BackgroundImageNode.AddColor = value.AsVector3Color();
        }
    }

    public Vector4 BorderColor {
        get => new(BorderImageNode.AddColor.X, BorderImageNode.AddColor.Y, BorderImageNode.AddColor.Z, BorderImageNode.InternalResNode->Color.A / 255.0f);
        set {
            BorderImageNode.InternalResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            BorderImageNode.AddColor = value.AsVector3Color();
        }
    }

    public Vector4 BarColor {
        get => new(ProgressNode.AddColor.X, ProgressNode.AddColor.Y, ProgressNode.AddColor.Z, ProgressNode.InternalResNode->Color.A / 255.0f);
        set {
            ProgressNode.InternalResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
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
