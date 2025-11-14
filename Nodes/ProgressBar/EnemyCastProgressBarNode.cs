using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class EnemyCastProgressBarNode : SimpleComponentNode {
    
    [JsonProperty] public readonly NineGridNode BackgroundImageNode;
    [JsonProperty] public readonly NineGridNode ProgressNode;
    
    public EnemyCastProgressBarNode() {
        BackgroundImageNode = new SimpleNineGridNode {
            NodeId = 2,
            TexturePath = "ui/uld/PartyList_GaugeCast.tex",
            TextureSize = new Vector2(204.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 12.0f),
            LeftOffset = 20,
            RightOffset = 20,
        };
        BackgroundImageNode.AttachNode(this);

        ProgressNode = new SimpleNineGridNode {
            NodeId = 3,
            TexturePath = "ui/uld/PartyList_GaugeCast.tex",
            TextureSize = new Vector2(188.0f, 7.0f),
            TextureCoordinates = new Vector2(8.0f, 3.0f),
            LeftOffset = 10,
            RightOffset = 10,
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
            BackgroundImageNode.InternalResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            BackgroundImageNode.AddColor = value.AsVector3Color();
        }
    }

    public Vector4 BarColor {
        get => new(ProgressNode.AddColor.X, ProgressNode.AddColor.Y, ProgressNode.AddColor.Z, ProgressNode.InternalResNode->Color.A / 255.0f);
        set {
            ProgressNode.InternalResNode->Color = new Vector4(1.0f, 1.0f, 1.0f, value.W).ToByteColor();
            ProgressNode.AddColor = value.AsVector3Color();
        }
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundImageNode.Size = Size;
        ProgressNode.Size = Size;
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
    }
}
