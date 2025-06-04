using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class ProgressBarNode : NodeBase<AtkResNode> {
    [JsonProperty] private readonly SimpleNineGridNode backgroundImageNode;
    [JsonProperty] private readonly SimpleNineGridNode progressNode;
    [JsonProperty] private readonly SimpleNineGridNode borderImageNode;

    public ProgressBarNode() : base(NodeType.Res) {
        InternalNode->SetWidth((ushort)160.0f);
        InternalNode->SetHeight((ushort)20.0f);
        
        backgroundImageNode = new SimpleNineGridNode {
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
        
        backgroundImageNode.AttachNode(this);

        progressNode = new SimpleNineGridNode {
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
        
        progressNode.AttachNode(this);

        borderImageNode = new SimpleNineGridNode {
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
        
        borderImageNode.AttachNode(this);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            backgroundImageNode.Dispose();
            progressNode.Dispose();
            borderImageNode.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public float Progress {
        get => progressNode.Width / Width;
        set => progressNode.Width = Width * value;
    }

    public override float Width {
        get => base.Width;
        set {
            backgroundImageNode.Width = value;
            progressNode.Width = value;
            borderImageNode.Width = value;
            base.Width = value;
        }
    }

    public override float Height {
        get => base.Height;
        set {
            backgroundImageNode.Height = value;
            progressNode.Height = value;
            borderImageNode.Height = value;
            base.Height = value;
        }
    }

    public Vector4 BackgroundColor {
        get => backgroundImageNode.Color;
        set => backgroundImageNode.Color = value;
    }

    public Vector4 BarColor {
        get => progressNode.Color;
        set => progressNode.Color = value;
    }

    public override void DrawConfig() {
        base.DrawConfig();

        using (var background = ImRaii.TreeNode("Background")) {
            if (background) {
                backgroundImageNode.DrawConfig();
            }
        }

        using (var progress = ImRaii.TreeNode("Progress")) {
            if (progress) {
                progressNode.DrawConfig();
            }
        }

        using (var border = ImRaii.TreeNode("Border")) {
            if (border) {
                borderImageNode.DrawConfig();
            }
        }
    }
}