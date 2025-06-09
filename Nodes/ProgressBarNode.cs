using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class ProgressBarNode : NodeBase<AtkResNode> {
    [JsonProperty] protected readonly SimpleNineGridNode BackgroundImageNode;
    [JsonProperty] protected readonly SimpleNineGridNode ProgressNode;
    [JsonProperty] protected readonly SimpleNineGridNode BorderImageNode;

    public ProgressBarNode() : base(NodeType.Res) {
        InternalNode->SetWidth((ushort)160.0f);
        InternalNode->SetHeight((ushort)20.0f);
        
        BackgroundImageNode = new SimpleNineGridNode {
            NodeId = 100,
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

        ProgressNode = new SimpleNineGridNode {
            NodeId = 200,
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

        BorderImageNode = new SimpleNineGridNode {
            NodeId = 300,
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
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            BackgroundImageNode.Dispose();
            ProgressNode.Dispose();
            BorderImageNode.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public float Progress {
        get => ProgressNode.Width / Width;
        set => ProgressNode.Width = Width * value;
    }

    public override float Width {
        get => base.Width;
        set {
            BackgroundImageNode.Width = value;
            ProgressNode.Width = value;
            BorderImageNode.Width = value;
            base.Width = value;
        }
    }

    public override float Height {
        get => base.Height;
        set {
            BackgroundImageNode.Height = value;
            ProgressNode.Height = value;
            BorderImageNode.Height = value;
            base.Height = value;
        }
    }

    public Vector4 BackgroundColor {
        get => BackgroundImageNode.Color;
        set => BackgroundImageNode.Color = value;
    }

    public Vector4 BarColor {
        get => ProgressNode.Color;
        set => ProgressNode.Color = value;
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